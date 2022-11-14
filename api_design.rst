==============================================================
                              API
==============================================================

JWT-Token
=========
Type: string (Extended ASCII)
Location: Header[Authorization]
Format:
    Bearer <token>

Schemas
=======
Seller
~~~~~~
API
---
::

    {
        id: string,
        name: string,
        rate: string,
        balance?: string
        revenue?: string 
        provision?: string 
        // rate and balance are strings to prevent floating-point imprecision
        // balance, revenue, provision may be missing in client requests (POST /seller and ...)
    }

Frontend (C#)
--------------
::

    Trader (class)
    + TraderID: string
    + Name: string
    + Balance: decimal
    - _ProvisionRate: decimal
    + Provision: decimal

+ Property ProvisionRate: decimal (_ProvisionRate~100)

SellerModify
~~~~~~~~~~~~
API
---
::

    {
        name?: string,
        rate?: string
        // Any of these may be missing, in which case they are not changed
    }


Item
~~~~
API
---
::

    {
        sellerId: string,
        price: string
    }


Frontend (C#)
-------------
::

    TransactionItem (class)
    + Trader: string
    + Price: decimal

MalformedTransaction
~~~~~~~~~~~~~~~~~~~~
API
---
::

    {
        "{item_index}" : [
            JSONArray[string], // Contains all non-parsable keys
            JSONArray[string] // Contains keys of all invalid values
        ],
        \...
    }


Endpoints
=========
POST /seller
~~~~~~~~~~~~~~~~~~~~~~~~~~~
Body: Schema/Seller

Responses
---------------------------
+ 201 Created
    + Content-Type: text/json
    + Content: Schema/Seller
+ 400 Bad Request
    + Content-Type: text/plain
    + Content: "Bad Request"
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "Bearer token is invalid"
+ 409 Conflict
    + Content-Type: text/plain
    + Content: "A seller with this name already exists"

GET /sellers
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Responses
---------------------------
+ 200 OK
    + Content-Type: text/json
    + Content: JSONArray[Schema/Seller]
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "Bearer token is invalid"

GET /seller/{id}
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Responses
---------------------------
+ 200 OK
    + Content-Type: text/json
    + Content: Schema/Seller
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "Bearer token is invalid"
+ 404 Not Found
    + Content-Type: text/plain
    + Content: "A seller with the id {id} doesn't exist"

DELETE /seller/{id}
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Responses
---------------------------
+ 200 OK
    + Content-Type: text/json
    + Content: Schema/Seller
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "Bearer token is invalid"
+ 403 Forbidden
    + Content-Type: text/plain
    + Content: "Seller balance is non-null. May not delete"
+ 404 Not Found
    + Content-Type: text/plain
    + Content: "A seller with the id {id} doesn't exist"

PATCH /seller/{id}
~~~~~~~~~~~~~~~~~~~~~~~~~~~
Body: Schema/SellerModify

Responses
---------------------------
+ 200 OK
    + Content-Type: text/json
    + Content: Schema/Seller
+ 400 Bad Request
    + Content-Type: text/plain
    + Content: "Key {key} is invalid and cannot be changed"
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "Bearer token is invalid"
+ 404 Not Found
    + Content-Type: text/plain
    + Content: "A seller with the id {id} doesn't exist"

POST /sell
~~~~~~~~~~~~~~~~~~~~~~~~~~~
Body: JSONArray[Schema/Item]

Responses
---------
+ 200 OK
    + Content-Type: text/json
    + Content: JSONArray[Schema/Seller] 
    + // These are all sellers relevant to the transaction with updated data
+ 400 Bad Request
    + Content-Type: text/json
    + Content: Schema/MalformedTransaction
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "Bearer token is invalid"

POST /login
~~~~~~~~~~~~~~~~~~~~~~~~~~~
Body:
    + Content-Type: text/plain
    + // This is just the password the user entered

Responses
---------
+ 200 OK
    + Content-Type: text/plain
    + Content: JWT-Token/Format
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "The password you entered is incorrect"

GET /exportcsv
~~~~~~~~~~~~~~~~~~~~~~~~~~~
+ 200 OK
    + Content-Type: text/csv
    + Content: Database exported as CSV-file. (Intended for confirmation)
+ 401 Unauthorized
    + Content-Type: text/plain
    + Content: "Bearer token is invalid"

GET /teapot
~~~~~~~~~~~~~~~~~~~~~~~~~~~
Responses
---------------------------
+ 418 I'm a teapot
