Feature: PV2-2083-Data-Locks-prevented-from-appearing-on-the-Data-Match-Report
Scenario: PV2-2083-Data-Locks-prevented-from-appearing-on-the-Data-Match-Report

Given two potential matching apprenticeships
And one which fails start date validation but who's start date is later than the second
And a second one which passes start date validation but is stopped
When the ILR is submitted
Then A DLock 10 is correctly generated

