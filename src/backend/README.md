# Backend Source Code

This directory contains the application's backend source code

## Services

After running `docker-compose up` from the project root, these services should be available:

| Name                  | URL                                 
| --------------------- | --------------------------------------------
| oracle-data-interface | http://localhost:5010/swagger-ui/index.html
| TrafficCourts         | http://localhost:5000/swagger/index.html    

## Description

### oracle-data-interface
An API that acts as an interface between Oracle and the TrafficCourts API 

### TrafficCourts
An API for creating violation ticket disputes
