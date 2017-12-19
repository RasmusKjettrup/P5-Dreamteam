# Documentation

WarehouseAI (C#):	Open CSDoc/html/index.html in a browser.
QR_reader (Android):	Open AndroidDoc/index.html and ignore R.*.
Crawler:		Only code in amazon-related crawler. Can possibly execute using Python 3 and Scrapy.


#Execution

QR_reader:		Move app-debug.apk to an Android phone and install. Grant access to camera if asked.
WarehouseAI:		Execute WarehouseAI_exe/WarehouseAI.exe and do the following (has sort of tab-completion):
				1. Make sure both Android phone and Windows PC are on same, local network (not AAU's networks)
				2. importwarehouse wh
				3. importitems it
				4. importrelations re
				5. importisbn isbn
				6. addbooks in console or using Android phone to send ID or scan ISBN of existing items
				7. orderbooks with list of ids in console
				8. Press "Request Order" in Android and get route