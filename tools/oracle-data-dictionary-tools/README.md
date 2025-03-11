# Oracle Data Dictionary Tools

Helper programs to read data from Oracle data dictionary and process it for various reasons.


## CrudGenerator

This tool helps generating C# and PL/SQL code to process CRUD type operations in Oracle.
This tool helps due to requirement that TCO use ORDS to access data.  The TCO project
is unable to use EF or direct database connections to perform this operations.

This program can generate these types of objects:

1. C# Models
1. CRUD PL/SQL packages that perform insert, update and delete operations using `%rowtype` parameters
1. Triggers - standardized triggers that fetch the `ID` primary key from value from sequence and sets audit fields

We have a configuration file for both schema, these files are `occam.json` and `tco.json`.

Program arguments:

```
  -f, --file      Required. The configuration file

  -o, --output    Required. The output path

  --sid           Required. The Oracle database sid

  --host          Required. The Oracle database host

  --username      Required. The Oracle database username

  --password      Required. The Oracle database password

  --trigger       Generate triggers

  --crud          Generate package for crud operations

  --model         Generate C# models

  --help          Display this help screen.

  --version       Display version information.
```

## DependencyGenerator

This tool helps to determine the dependency order of objects and determine the file names of the Oracle objects.

It will find all package, package body, procedure, function, and trigger objects. Determine which objects depend on 
which other objects. It only handles checking for dependencies within the same schema. Once the order is determined,
the program attempts to determine the file name for the object. It will try to find the name from the SVN keyword $HeadURL: $
string. If the object does not contain that text, then it will search all the files in the given path for a file that 
`creates or replace` the object.

```
  --path          Required. The path to search for files with create or replace statements. Only used if SVN keyword $HeadURL: $ is not found

  --sid           Required. The Oracle database sid

  --host          Required. The Oracle database host

  --username      Required. The Oracle database username

  --password      Required. The Oracle database password


  --help          Display this help screen.

  --version       Display version information.
```

## ORacle.DbmlGeneration  - Oracle To DBML

This tools helps generate DBML diagrams basedon the Oracle data dictionary

### Configuration

Sample User Secrets configuration:


```json
{
    "AppConfiguration": {
        "host": "localhost",
        "sid": "freeepdb1",
        "occam": {
            "username": "username",
            "password": "password"
        },

        "tco": {
            "username": "username",
            "password": "password"
        }
    }
}

```
