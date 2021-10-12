# Error types

Error              | HTTP status code   |Description
------------------ | -----| --------------------------------------------------------
ETNFND001          | 404  | Requested entity hasn't been found 
FORBID001          | 403  | Access to the requested resource is forbidden 
GTWAY001           | 502  | Bad gateway
GTWAY002           | 504  | Gateway sent time out during request 
NFOUND001          | 404  | Requested resource hasn't been found
SRVRER001          | 500  | General server error while executing request
UNVALI001          | 400  | The parameters used with the request are invalid 