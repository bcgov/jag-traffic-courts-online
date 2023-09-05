#Traffic Court Online Violation Ticket Generator
This Java project will generate a Violation Ticket useful in testing, either with randomly-generated or specific values.

## How to build
From the /tools folder,
 
`docker-compose up -d --build ticket-gen`

## Swagger
Visit [http://localhost:8080/swagger-ui/index.html](http://localhost:8080/swagger-ui/index.html)

The main endpoint is `/generate`.

Issue a GET request, specify a writingStyle between 1 and 8
[http://localhost:8080/generate?writingStyle=5](http://localhost:8080/generate?writingStyle=5)
```
curl -X 'GET' \
  'http://localhost:8080/generate?writingStyle=5' \
  -H 'accept: image/png'
```

Issue a POST request to generate an image with specific values.
```
curl -X 'POST' \
  'http://localhost:8080/generate?writingStyle=5' \
  -H 'accept: image/png' \
  -H 'Content-Type: application/json' \
  -d '{
  "violationTicketNumber": "AX00000000",
  "surname": "Kent",
  "givenName": "Clark",
  "isYoungPerson": "X",
  "driversLicenceProvince": "BC",
  "driversLicenceNumber": "1234567",
  "driversLicenceCreated": "2020",
  "driversLicenceExpiry": "2025",
  "birthdate": "2006-01-15",
  "address": "123 Small Lane",
  "isChangeOfAddress": "",
  "city": "Smallville",
  "province": "BC",
  "postalCode": "V9A1L8",
  "namedIsDriver": "X",
  "namedIsCyclist": "",
  "namedIsOwner": "",
  "namedIsPedestrain": "",
  "namedIsPassenger": "",
  "namedIsOther": "",
  "namedIsOtherDescription": "",
  "violationDate": "2022-02-24",
  "violationTime": "15:23",
  "violationOnHighway": "Smallville Bypass",
  "violationNearPlace": "Kent Farm",
  "offenseIsMVA": "X",
  "offenseIsMCA": "",
  "offenseIsCTA": "",
  "offenseIsWLA": "",
  "offenseIsFAA": "",
  "offenseIsLCA": "",
  "offenseIsTCR": "",
  "offenseIsOther": "",
  "offenseIsOtherDescription": "",
  "count1Description": "Excessive speeding",
  "count1ActReg": "MVA",
  "count1IsACT": "X",
  "count1IsREGS": "",
  "count1Section": "67(b)",
  "count1TicketAmount": "350",
  "count2Description": "Driving without licence",
  "count2ActReg": "MVA",
  "count2IsACT": "X",
  "count2IsREGS": "",
  "count2Section": "45(a)",
  "count2TicketAmount": "145",
  "count3Description": "Driving with burned out break lights",
  "count3ActReg": "MVA",
  "count3IsACT": "X",
  "count3IsREGS": "",
  "count3Section": "124(c)(i)",
  "count3TicketAmount": "75",
  "vehicleLicensePlateProvince": "BC",
  "vehicleLicensePlateNumber": "123ABC",
  "vehicleNscPuj": "AABB",
  "vehicleNscNumber": "12345",
  "vehicleRegisteredOwnerName": "Jonathan Kent",
  "vehicleMake": "John Deer",
  "vehicleType": "Tracker",
  "vehicleColour": "Green",
  "vehicleYear": "2011",
  "noticeOfDisputeAddress": "123 Main St.",
  "hearingLocation": "Metropolis Court",
  "dateOfService": "2022-02-24",
  "enforcementOfficerNumber": "12345",
  "detachmentLocation": "Metropolitan Police"
}'
```