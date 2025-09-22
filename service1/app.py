from fastapi import FastAPI, Depends, HTTPException
from fastapi.responses import Response
import datetime
import time
import shutil 
 
app = FastAPI(title="service1")

start_time = time.time() #To store service starting time

def analyze_status():
    # implementation of status analysis
       
    # Get current timestamp
    timestamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

    # Calculate uptime  in hours with 2 decimal places
    uptime_hours = round((time.time() - start_time) / 3600, 2)

    # Calculate free disk space in root
    total, used, free = shutil.disk_usage("/")
    free_disk_space_mb = free // (1024 * 1024)

    # Define the status record
    status_record = f"{timestamp}: uptime {uptime_hours} hours, free disk in root: {free_disk_space_mb} MBytes"
     
    return status_record

def log_status_storage(status_record):
    # logging to storage service
    print(f"Logging to storage: {status_record}")
    
def log_status_vstorage(status_record):
    # logging to vstorage
    print(f"Logging to vstorage: {status_record}")
    
 
@app.get("/status", response_class=Response)
async def read_status():
    """Status endpoint to analysis the status and log the status to log file and vstorage."""
    print("Status endpoint called")
    status_record1 = analyze_status()
    log_status_storage(status_record1)
    log_status_vstorage(status_record1)
    
    status_record2 = analyze_status() # from service 2 TODO: Replace with actual call to service 2
    combined_response = f"{status_record1}\n{status_record2}"
    print(f"Combined status: {combined_response}")
    return combined_response
   

@app.get("/log")
async def log():
    """Log endpoint to forward the request to staorage service and retrive the contents of the log."""
    print("Log endpoint called")
    log_records = analyze_status() # forward to staorage service: Replace with actual call to storage 
    
    return log_records
