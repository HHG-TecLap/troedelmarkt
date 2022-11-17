==============================================================
                        Database Layout
==============================================================

Table: seller
=============
+ id: TEXT PRIMARY KEY NOT NULL
+ name: TEXT NOT NULL
+ rate: TEXT
+ balance: TEXT NOT NULL DEFAULT "0"
+ revenue: TEXT NOT NULL DEFAULT "0"
+ provision: TEXT NOT NULL DEFAULT "0"

*Note: Many of these could also be REALs, but are not to increase accuracy*