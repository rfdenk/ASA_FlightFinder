
 1) This project was written using VS2015 as a 4.6.2 .NET server without cloud deployment.

 2) This project does make use of WebGrid, which may or may not be included with your IDE.

 3) The primary controller for this project is FlightSearchController.cs.

 4) If a flight occurs at a time that is before "now", the server will convert its time to tomorrow.

 5) If, in ListFlights, you manipulate the URL to include only FromAirport or only ToAirport or neither, then
    the server will redirect back to FindFlights, with the non-missing parameter pre-selected into the drop-down.

 6) You can find the JSON endpoints at /FlightSearch/JSON/ListAllAirports and /FlightSearch/JSON/ListMatchingFlights
      i) ListMatchingFlights takes two parameters, both of which are optional: FromAirport and ToAirport, like this:
         /FlightSearch/JSON/ListMatchingFlights?FromAirport=LAX&ToAirport=SEA.
     ii) If a parameter is omitted from ListMatchingFlights, then the server will ignore it; 
         e.g., "ListMatchingFlights?FromAirport=LAS" will show all flights _from_ LAS.

 7) There are a few tests, but they are not heavily populated. The csv files used by the main server are also present
    in the testing folder, and, during testing, these files are used. Therefore, testing can use specific "test-only"
    files.
