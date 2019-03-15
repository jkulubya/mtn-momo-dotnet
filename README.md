# MTN Momo API for .NET
[![Build status](https://ci.appveyor.com/api/projects/status/60rfh0wkdhh68jn0?svg=true)](https://ci.appveyor.com/project/CJ96122/mtn-momo-dotnet)

Use the [MTN Mobile Money API](https://momodeveloper.mtn.com) in your .NET applications.

Currently targeting netstandard1.3.

## Installation
Find the package on [NuGet](https://www.nuget.org/packages?q=mtnmomo.net)

Visual Studio Nuget Console
```
Install-Package MtnMomo.NET -IncludePrerelease
```

Dotnet CLI Tools
```
dotnet add package MtnMomo.NET --version 1.0.0-alpha-17
```

## Usage

### Collections
[Documentation](https://momodeveloper.mtn.com/docs/services/collection/operations/requesttopay-POST)

```
using MtnMomo.NET;

var config = new MomoConfig();
config.UserId = UserId;
config.UserSecret = UserSecretKey;
config.SubscriptionKeys.Collections = SubscriptionKey;

var momo  =  new Momo(config);
var collections = momo.Collections;

var result = await collections.RequestToPay(
    25000.00M,
    "UGX",
    "External ID",
    new Party("0777000000", PartyIdType.Msisdn),
    "Payer message",
    "Payer note",
    new Uri("http://www.example.com")
);
```

### Disbursements
[Documentation](https://momodeveloper.mtn.com/docs/services/disbursement/operations/token-POST)

```
using MtnMomo.NET;

var config = new MomoConfig();
config.UserId = UserId;
config.UserSecret = UserSecretKey;
config.SubscriptionKeys.Disbursements = SubscriptionKey;

var momo = new Momo(config);
var disbursements = momo.Disbursements;

var result = await disbursements.Transfer(
    25000.00M,
    "UGX",
    "External ID",
    new Party("0777000000", PartyIdType.Msisdn),
    "Payer message",
    "Payee message",
    new Uri("http://www.example.com")
);
```

### Remittances
[Documentation](https://momodeveloper.mtn.com/docs/services/remittance/operations/token-POST)

```
using MtnMomo.NET;

var config = new MomoConfig();
config.UserId = UserId;
config.UserSecret = UserSecretKey;
config.SubscriptionKeys.Remittances = SubscriptionKey;

var momo = new Momo(config);
var remittances = momo.Remittances;

var result = await remittances.Transfer(
    25000.00M,
    "UGX",
    "External Id",
    new Party("0777000000", PartyIdType.Msisdn),
    "Payer note",
    "Payee note",
    new Uri("http://www.example.com")
);
```
