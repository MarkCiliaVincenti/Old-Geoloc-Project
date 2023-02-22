# SmartFleet
SmartFleet is an Open source solution supports teltonika and tk103 protocols 
- This solution  can be deployed on Azure cloud. 
- It is a distributed system that supports  AMQP protocol (for managing the comming data from boxes ) and stores  data on the MicroSoft server database.
- The web solution consists of two parts :
- one for administration management, where you can add users, customers and vehicles to sing-in to the this part just use: admin@smartFleet/123456 (we work on it because it's incomplete yet).
- the seconde part is for customers where:
- add zone of interest to the map from setting icon. 
- You can  track your vehicles on run time.
- You can display the full itinerary on the map with the different activities over the time as a chronogram  along with a lot of informations such as speed, duration and addresses.
- generate reports by selecting periods from calender and saving them as a PDF file.
- We are working now for adding new features such as adding drivers, displaying fuel consumption reports and possibly supporting card drivers.
to sing-in to the this part just use: customer@smartFleet/123456.

# How to deploy the solution:
- after cloning the solution and  restoring  nuget packages , you have to create a database instance on Sql server, then change connection string from web.config in the web solution ( once done the solution creates the database for you on the start and seeds some data like roles and some users see : Global.asax in web soltuion).
- to  build the solution you have to install azure tools for Visual studio (the version used is Vs 2017 community ).
- when every thing is Ok you can run the application as azure service by selecting SmartFleet.Azure a start up projet , this project will run tow roles one is the tcp Server role that listening to boxes connections on prot 34400. and the web role.
- if you want to test the Server tcp  there is a small project called TeltonikaEmulator in test directory with some real data from real boxe (the file is attached as a resource ).
- if you are interested to this project and you want to contribute contact me at adgroupe@hotmail.com.

![](https://github.com/pentest30/SmartFleet/blob/master/src0.png)
![](https://github.com/pentest30/SmartFleet/blob/master/src1.png)
![](https://github.com/pentest30/SmartFleet/blob/master/src2.png)
![](https://github.com/pentest30/SmartFleet/blob/master/src3.png)
![](https://github.com/pentest30/SmartFleet/blob/master/src6.png)
![](https://github.com/pentest30/SmartFleet/blob/master/src4.png)
![](https://github.com/pentest30/SmartFleet/blob/master/src5.png)
