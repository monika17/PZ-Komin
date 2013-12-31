echo sending to Client...
@echo off
copy KominCipherSuite.cs ..\..\Client\Client\*
copy KominClientSideConnection.cs ..\..\Client\Client\*
copy KominDatabaseStructures.cs ..\..\Client\Client\*
copy KominNetworkJob.cs ..\..\Client\Client\*
copy KominNetworkPacket.cs ..\..\Client\Client\*
echo sending to sample_client...
copy KominCipherSuite.cs ..\sample_client\*
copy KominClientSideConnection.cs ..\sample_client\*
copy KominDatabaseStructures.cs ..\sample_client\*
copy KominNetworkJob.cs ..\sample_client\*
copy KominNetworkPacket.cs ..\sample_client\*
pause