
<p align="center">
    <a href="https://dev.azure.com/bmresearch/bonfidanet/_build/latest?definitionId=4&branchName=refs%2Fpull%2F7%2Fmerge">
        <img src="https://img.shields.io/azure-devops/build/bmresearch/bonfidanet/4/master?style=flat-square"
            alt="Azure DevOps Build Status (master)" ></a>
    <a href="https://img.shields.io/azure-devops/coverage/bmresearch/bonfidanet/4/master">
        <img src="https://img.shields.io/azure-devops/coverage/bmresearch/bonfidanet/4/master?style=flat-square"
            alt="Azure DevOps Cobertura Code Coverage (master)"></a>
    <a href="https://github.com/bmresearch/Bonfidanet/blob/master/LICENSE">
        <img src="https://img.shields.io/github/license/bmresearch/Bonfidanet?style=flat-square" 
            alt="Code License"></a>
    <br>
    <a href="https://twitter.com/intent/follow?screen_name=blockmountainio">
        <img src="https://img.shields.io/twitter/follow/blockmountainio?style=flat-square&logo=twitter"
            alt="Follow on Twitter"></a>
</p>

# What is Bonfidanet?

Bonfidanet is a .NET library to interface with the [Bonfida API](https://docs.bonfida.com/).

## Features
- Full HTTP API Coverage
- Full WebSocket API Coverage

## Requirements
- net 5.0

## Examples

The [Bonfidanet.Examples](https://github.com/bmresearch/Bonfidanet/src/Bonfida.Examples/) project contains some code examples, but essentially we're trying very hard to
make it intuitive and easy to use the library.

### Interfacing with the HTTP API

```c#
// To interface with the HTTP API get the client from the factory
var client = ClientFactory.GetClient();

// Get all the available market pairs on the SERUM DEX
var marketPairs = client.GetAllPairs();
``` 

### Interfacing with the WebSocket API

```c#
// To interface with the HTTP API get the client from the factory
var client = ClientFactory.GetStreamingClient();

// Subscribe to serum trades feed
client.SubscribeTrades(trade => {
    Console.WriteLine($"Trade - Market: {trade.Market} Price: {trade.Price} Size: {trade.Size}");
});
``` 



## Contribution

We encourage everyone to contribute, submit issues, PRs, discuss. Every kind of help is welcome.

## Contributors

* **Hugo** - *Maintainer* - [murlokito](https://github.com/murlokito)

See also the list of [contributors](https://github.com/bmresearch/Bonfidanet/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Bonfidanet/blob/master/LICENSE) file for details
