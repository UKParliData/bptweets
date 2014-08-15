Briefing Paper Tweets
=====================

This is a small sample application to demonstrate the capabilities of the data
feeds available from www.data.parliament.uk. It polls the Briefing Papers feed
from http://lda.data.parliament.uk/briefingpapers and tweets whenever a new
Briefing Paper is published.

Usage
-----

 1. Log into https://apps.twitter.com/ and create a new application. Leave the
    callback URL blank but supply all the other requested information.
 2. The application will need read and write access permissions.
 3. Copy the API key and API secret into the program's app.config file.
 4. Run the application with the /a parameter (to authorise):

    bptweets /a

 5. Enter the PIN number that Twitter gives you into the console.
 6. The application will then automatically reconfigure itself to use the new
    authorisation codes.
 7. Run the application at the console with no parameters:

    bptweets

 8. Set it up as a scheduled task to run every so often to poll for new briefing
    papers and post to Twitter.

Note
----

From time to time, the length of shortened Twitter URLs may change. You should
check the setting in app.config periodically and adjust when necessary.

You could make the application poll Twitter and update this value automatically.
This is left as an exercise for the reader.
