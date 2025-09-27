# DEVOPS - exercise1 (Service1, Service2, Storage)

# Exercise 1 – Multi-Service System with Persistent Storage

This repository contains a simple system with three services, implemented in **different technologies**:

## Requirements

- **Docker** ≥ 20.x  
- **Docker Compose** ≥ 2.x  
- Git  
- .NET 9 SDK (for building Service2 before containerizing, if required) 

## System Architecture

- **Service1** – Python (FastAPI)  
  - The **only service accessible from outside**.  
  - Collects system uptime and free disk space.  
  - Forwards requests to **Service2** and **Storage**.  
  - Stores logs in:
    - Shared file volume `vstorage`
    - Storage service (via REST API) 
  - Endpoints:
    - `GET /status` → Analyze uptime , space and Append new log entry (`text/plain`)
    - `GET /log` → Retrieve all logs (`text/plain`)

- **Service2** – .NET 9 Web API  
  - Collects system uptime and free disk space.  
  - Logs records in both `vstorage` and the Storage service.  
  - Returns status info back to Service1. 
  - Endpoints:
    - `GET /status` → Analyze uptime , space and Append new log entry (`text/plain`) 

- **Storage** – Node.js (Express)  
  - Simple REST API for persistent logs.  
  - storage_data: a named Docker volume mounted only into Storage at `/app/data` and used to persist its internal `log.txt`.
  - Persists logs between container restarts using Docker volumes.  
  - Endpoints:
    - `POST /log` → Append new log entry (`text/plain`)
    - `GET /log` → Retrieve all logs (`text/plain`)

- **vstorage** – host file `./vstorage` mounted into Service1 and Service2 at `/app/vstorage` and appended per request. This is the simple shared file method. 
 
- **Networking** – 
- A single user-defined `services_network` network connects all three services. Only Service1 publishes a host port.


## How to Run
 
- Clone the repository using command: `git clone -b exercise1 https://github.com/LokeswariMelapattu/devops-course.git`
- `cd devops-course`
- Build and start: `docker-compose up --build -d`
- Wait ~10 seconds
- Test status flow: `curl localhost:8199/status`
- Fetch first storage log : `curl localhost:8199/log`
- Fetch second storage log : `cat ./vstorage`
- Stop: `docker-compose down`

cleanup instructions
- Clean the logs from host-file storage: `> ./vstorage`
- Remove the named volume for Storage: `docker volume rm devops-course_storage_data`

