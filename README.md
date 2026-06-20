mTLSTest
===========

This repository contains tools for testing mTLS connectivity with a TAG Neuron(R). It contains
two main components:

* A simple web page and a simple web service that can be used to test mTLS connectivity. These
can be placed in a distributable package and be installed on the Neuron where you want to test 
mTLS connectivity. The web page will display simple information available about the client for
a human user. The web service will return the same information in a JSON encoded object,
permitting automated testing.

* A simple console application that can be run in .NET Core environments. It is written using
.NET Standard for maximum portability. using command-line arguments it allows a tester to
automate testing of mTLS connectivity. It also provides a simple application that can be used
as a template, or to troubleshoot mTLS connectivity issues.
