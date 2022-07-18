import { sleep, check, group } from 'k6'
import http from 'k6/http'
import { Trend } from 'k6/metrics';

const host = `${__ENV.TARGET_HOST}`;
const apiVersion = `${__ENV.API_VERSION}`;
const modelId = `${__ENV.MODEL_ID}`;
const subscriptionKey = `${__ENV.KEY}`;
const ticketImage = `${__ENV.TICKET_IMAGE}`;
const includeTextDetails = true;

const ticketFile = open(ticketImage, 'b');
const ticketUrl = `${__ENV.TICKET_URL}`;
    
const analyzeTime = new Trend('analyze_time', true);

export const options = {
    thresholds: {},
    scenarios: {
        Analyze_Violation_Ticket: {
            executor: 'ramping-vus',
            gracefulStop: '120s',
            stages: [
                 { target: 1, duration: '1s' },
            //     { target: 20, duration: '3m30s' },
            //     { target: 0, duration: '1m' },
            ],
            startVUs: 1,
            gracefulRampDown: '30s',
            exec: 'analyze_violation_ticket',
        },
    },
}

export function analyze_violation_ticket() {
    let response;

    var analyzeRequestUrl = (apiVersion === 'v2.1') ?
        `${host}/formrecognizer/${apiVersion}/custom/models/${modelId}/analyze?includeTextDetails=${includeTextDetails}` :
        `${host}/formrecognizer/documentModels/${modelId}:analyze?api-version=${apiVersion}`;
    var analyzeContentType = (apiVersion === 'v2.1') ? 'image/png' : 'application/json'
    
    console.log("analyzeRequestUrl:" + analyzeRequestUrl);
    
    group('Analyze Violation Ticket', function () {
        var body = (ticketUrl == 'undefined') ? ticketFile : ticketUrl;
        // Create Analyze Job
        response = http.post(
            analyzeRequestUrl,
            body,
            {
                headers: {
                    'content-type': analyzeContentType,
                    accept: 'application/json',
                    'Ocp-Apim-Subscription-Key' : subscriptionKey
                },
                tags: {
                    name: 'analyze'
                },
            }
        )

        if (check(response, { 'status equals 202': response => response.status.toString() === '202' })) {            
            // operation location will have internal urls, not ones exposed through route
            var operationLocation = response.headers['Operation-Location']
            
            // extract the job id
            var lastSlash = operationLocation.lastIndexOf('/'); 
            var resultId = operationLocation.substring(lastSlash+1);
            
            var analyzeResultsUrl = (apiVersion === 'v2.1') ?
                `${host}/formrecognizer/${apiVersion}/custom/models/${modelId}/analyzeresults/${resultId}` :
                `${host}/formrecognizer/documentModels/${modelId}/analyzeResults/${resultId}`; // resultId includes the api-version parameter
            
            console.log("analyzeResultsUrl:" + analyzeResultsUrl);
            
            let done = false;
            let retries = 0
            while (!done && retries < 5) {
                // Check Analyze Job
                response = http.get(analyzeResultsUrl,
                    {
                        headers: {
                            accept: 'application/json',
                            'Ocp-Apim-Subscription-Key' : subscriptionKey
                        },
                        tags: {
                            name: 'check-operation-status'
                        }
                    }
                );

                if (check(response, { 'status equals 200': response => response.status.toString() === '200' })) {
                    const json = response.json();
                    if (json.status === 'succeeded') {
                        const createdDateTime = Date.parse(json.createdDateTime);
                        const lastUpdatedDateTime = Date.parse(json.lastUpdatedDateTime);
                        const elapsed = lastUpdatedDateTime - createdDateTime;
                        analyzeTime.add(elapsed);
                        done = true;   
                    } else {
                        sleep(1);
                    }
                } else {
                    retries = retries + 1;                
                    console.error(response);
                }
            }

            console.log("retries:" + retries);
        }
    })
}
