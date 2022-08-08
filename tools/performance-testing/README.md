# Performance Testing

1. Install k6
1. Copy a PNG called violation-ticket.png into this folder.
1. Run test

To run against a local or OpenShift deployed instance, version 2.1
```
k6 run 
-e TARGET_HOST="http://localhost:5200" ^
-e API_VERSION=v2.1 ^
-e MODEL_ID=75f5614c-eded-4413-a3d9-c67281e8402e ^
-e TICKET_IMAGE=violation-ticket-small.png ^
test-formrecognizer.js
```

To run against Azure Services API, version 2022-06-30-preview
```
k6 run ^
-e TARGET_HOST="https://westus2.api.cognitive.microsoft.com" ^
-e API_VERSION=2022-06-30-preview ^
-e MODEL_ID=ViolationTicket_v4 ^
-e TICKET_URL="{'urlSource': 'http://public_url_to_png'}" ^
-e KEY=__subscription_key__ ^
-e TICKET_IMAGE=violation-ticket-small.png ^
test-formrecognizer.js
```
