from fastapi import FastAPI, Depends, HTTPException
from fastapi.responses import Response
import datetime
import time
import shutil 
import requests
import os
 
app = FastAPI(title="service1")

start_time = time.time() #To store service starting time
service2_url = "http://service2:8080/status" # URL of service 2
storage_url = "http://storage:8082/log" # URL of storage service
vstorage_path = "/vstorage" # Path to vstorage log file

def analyze_status():
    # implementation of status analysis
       
    # Get current timestamp
    timestamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

    # Calculate uptime  in hours with 2 decimal places
    uptime_hours = (time.time() - start_time) / 3600

    print(f"Uptime hours: {uptime_hours}")
    
    # Calculate free disk space in root
    total, used, free = shutil.disk_usage("/")
    free_disk_space_mb = free // (1024 * 1024)

    # Define the status record
    status_record = f"{timestamp}: uptime {uptime_hours:.2f} hours, free disk in root: {free_disk_space_mb} MBytes"
     
    return status_record

def log_status_storage(status_record):
    # logging to storage service
    print(f"Logging to storage: {status_record}")
    
    try:
        response = requests.post(storage_url, data=status_record, headers={"Content-Type": "text/plain"}) 
        print("Log successfully sent to storage service") 
        response.raise_for_status()
        return response.status_code == 200
    except requests.RequestException as e:
        print(f"Failed to send log to storage service: {e}")
        return False
        
    
def log_status_vstorage(status_record):
    # logging to vstorage
    print(f"Logging to vstorage: {status_record}")
    try:
        with open(vstorage_path, "a") as f:
            f.write(status_record + "\n")
        print("Log successfully written to vstorage")
        return True
    except IOError as e:
        print(f"Failed to write log to vstorage: {e}")
        return False
    
 
@app.get("/status", response_class=Response)
async def read_status():
    """Status endpoint to analysis the status and log the status to log file and vstorage."""
    print("Status endpoint called")
    status_record1 = analyze_status()
    log_status_storage(status_record1)
    log_status_vstorage(status_record1)
    
    try:
        response = requests.get(service2_url)
        response.raise_for_status()
        status_record2 = response.text
        print(f"Received status from service2: {status_record2}")
    except requests.RequestException as e:
        print(f"Failed to get status from service2: {e}")
        status_record2 = "service2 status unavailable"
    
    combined_response = f"{status_record1}\n{status_record2}"
    print(f"Combined status: {combined_response}")
    return combined_response
   

@app.get("/log", response_class=Response)
async def log():
    """Log endpoint to forward the request to staorage service and retrive the contents of the log."""
    print("Log endpoint called")
    # forward to staorage service: Replace with actual call to storage 
    try:
        response = requests.get(storage_url)
        response.raise_for_status()
        log_records = response.text
        print("Log records retrieved from storage service")
    except requests.RequestException as e:
        print(f"Failed to retrieve log from storage service: {e}")
        log_records = "Failed to retrieve log records"
    
    return log_records
