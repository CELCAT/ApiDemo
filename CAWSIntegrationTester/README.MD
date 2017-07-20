# CELCAT Timetabler Web Service Integration Test Tool


This project shares the source code of the CELCAT Timetabler Web Service (CAWS) Integration Test Tool used by CELCAT internally.

The tool is created by the CELCAT Testing Team for testing purpose. Any issues and bugs in the source code doesn't reflect the code quality of CELCAT products.

The source code can be used as a reference demonstrating how to communicate with the CAWS endpoints including
 * How to obtain OAuth2 token
 * How to make requests to the service endpoints
 * How to work with the response

## How to use the source code

You can either download the source code directly or check out with any version control tools supporting Git.

Once the source code is downloaded to your local filesystem, you can open the project using Visual Studio.

To compile the source code, you need to add a reference to the Celcat.Model.dll. The file can be found at the installation folder of the CAWS, which typically is C:\Program Files (x86)\CELCAT\Attendance Web Services\bin

To run the source code, you need to install and configure the CAWS. Please follow CELCAT Wiki on how to do so.
