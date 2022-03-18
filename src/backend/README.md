# Backend Source Code

This directory contains the application's backend source code

## Services

After running `docker-compose up` from the project root, these services should be available:

| Name                  | URL                                          | Notes
| --------------------- | -------------------------------------------- | --------------------------------------------
| oracle-data-interface | http://localhost:5010/swagger-ui/index.html  | 
| TrafficCourts         | http://localhost:5000/swagger/index.html     | 
| Splunk                | http://localhost:8000                        | login with admin/password

## Description

### oracle-data-interface
An API that acts as an interface between Oracle and the TrafficCourts API 

### TrafficCourts
An API for creating violation ticket disputes

### Splunk
A logging collector tool to capture, index, and correlate logs in a searchable repository.

A sample query might be (to find all logs produced from the Oracle Interface API:
`source="oracle-data-interface"`
