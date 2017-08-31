# CELCAT Timetabler Web Service Integration Test Tool




## Disclaimer

The source code shared here is created by various teams in CELCAT with different programming skills and responsibilities, and therefore the code quality may vary accordingly. This doesn't reflect the code quality of CELCAT's Development team or its official products.

## Copyright

Unless specified otherwise, the copyright of all shared information (including source code, instructions and documents) is described in the LICENSE file. CELCAT generally grants the GNU (GNU's Not Unix) General Public License version 3, but the information shared by other contributors may be covered by the license of the contributor’s choice.

## Limitation of Liability

CELCAT is not liable for any damages caused by use of shared information. More details are described in Section 15, 16 and 17 of the GNU General Public License version 3.

## Support

None of the shared information (including source code, instructions and documents) is considered as CELCAT's official products and thus is neither supported by the CELCAT Tech Support team nor subject to the Service Level Agreement. Any issues or feature requests should be raised on the Issues section of the GitHub website - a GitHub account is needed.

## Introduction

This project shares the source code of the CELCAT Timetabler Web Service (CAWS) Integration Test Tool used by CELCAT internally to automatically test the CAWS product.

## Interesting Points

Although the project is to test the CAWS, technically it essentially is a client application consuming the Web API. Therefore the source code demonstrates how to communicate with the CAWS endpoints including
* How to obtain OAuth2 token
* How to make requests to the CAWS endpoints with the OAuth2 token
* How to work with the CAWS responses

## How to Download


The source code can be directly downloaded or checked out using any version control tools supporting Git or GitHub. 
Then the project can be opened using Visual Studio. Free editions of Visual Studio are available on the Microsoft website.

## How to Compile

A reference to the Celcat.Model.dll must be added to the project before the source code can be successfully compiled. The file can be found at the CAWS installation folder, typically at C:\Program Files (x86)\CELCAT\Attendance Web Services\bin

## How to Use

Compiling the source code will create a console executable. All the parameters are passed in via command line parameteres. Please launch a Command Line Window and run the executable without any parameters to see the usage.

This console application can only work with a CAWS service properly configured. Please check the CELCAT Wiki for more details on CAWS installation and configuration.
