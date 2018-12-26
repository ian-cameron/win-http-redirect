# WinHttpRedirect

## What?
Light weight Windows HTTP Server that responds to all requests with a 301 redirect response.  Installs as a configurable Windows Service.  Tiny memory footprint and multithreaded request handler.  Appends any request parameters to the redirect location URL.

## Why?
Why install IIS and redirects module when you only want to redirect everything?

## How?
Theres an Install .bat script in the Deployment directory.

Make configuration changes by editing the .config file. The following parameters are available

    IP - The IP for the interface the server listens on.  Or blank for any.
    Port - The TCP address to bind the server on.  Default is 80.
    Redirect - The URL to redirect requests to.  This is required to run.
