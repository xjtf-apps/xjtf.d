# xjtf.d api

GET /Monitor/Services : ServiceInfo[]
- gets the status of all windows service on the target machine

GET /Monitor/Service/{serviceId | serviceName} : ServiceInfoBonus?
- gets the status of a windows service on the target machine and related data

POST /Monitor/Service (@Body:ServiceEntry) : CreatedResult
- upserts a service entry to the database

POST /Monitor/Service/State/{serviceId | serviceName} (@Body:ServiceEntryState) : CreatedResult
- inserts a service entry state to the database

PUT /Monitor/Clutch (@Body:DaemonDettachState) : UpdatedResult
- attaches or dettaches the main service monitoring loop

GET /Monitor/Services/Subscribe : WebSocket
- accepts a web socket connection to subscribe to the status of all services

GET /Monitor/Service/Subscribe{serviceId | serviceName} : WebSocket
- accepts a web socket connection to subscribe to the status info of a service