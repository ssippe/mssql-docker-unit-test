# MSSQL Docker Unit Tests

This solution demonstrates:
* Using a DACPAC / SSDT  .sqlproj to create a .dacpac.
* Building the DACPAC in docker
* Creating a container image bases on the base mssql image and applying the DACPAC.
* Running parallel unit tests against the database container image.

## Build Image
To create / refresh the database container image run:

`docker build . --build-arg "ACCEPT_EULA=Y" --build-arg "SA_PASSWORD=123456a@" --build-arg "DB_NAME=testdb" -t ssippe/mssql-docker-unit-test:latest`

To run the container outside of unit tests:

`docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=123456a@" ssippe/mssql-docker-unit-test:latest`

## Run Tests
First build the image then run in the tests for DbUnitTest in Visual Studio or `DbUnitTest> dotnet test`



