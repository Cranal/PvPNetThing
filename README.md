# PvPNetThing
Install:
1) Install postgressql data base. With user postgress and password 53170 by default, or with any other
1.1) If non default password installed please set it in the dbConnect.xml file in server folder.
2) Create database pvpnetthing
3) Import Table.sql from Things folder into database to create folder.
4) Import PvPNetThngCertPrivateKey.pfx certificate into you Private certificates container.
5) Put TempCert.cer into server and client folders.
6) Run PvPNetServer.exe wait till it runs, then look at the IP to which it assigned itself.
7) Specify this IP in etc\hosts like this: <IP> PvPNetThing
8) Run PvPNetThing.exe
