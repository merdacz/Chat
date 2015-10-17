# Chat
[![Build status](https://ci.appveyor.com/api/projects/status/d099cxs58qa6pqr8?svg=true)](https://ci.appveyor.com/project/merdacz/chat)
[![Stories in Ready](https://badge.waffle.io/merdacz/Chat.png?label=ready&title=Ready)](https://waffle.io/merdacz/Chat)

Current master build always available at: http://chitchatter.azurewebsites.net/

## Summary
Sample implementation of web chat application. Uses Bootstrap for responsive UI and Knockout JS for View-ViewModel decoupling. On backend side ASP.NET MVC with SignalR is being used to enable real-time communication. For now embedded RavenDB is being used for persistence.

To run the project simply F5 it in Visual Studio. Note though  that first load may take a while due to RavenDB setting itself up.

## Domain layer
The main domain class is `Chat`, which models interaction with abstract chat room. It does not depend on any of technologies mentioned above. It is decoupled from database provider via `IMessageLog`, host configuration via `IChatConfiguration`. It is being used from SignalR hub `ChatHub` but on its own it is unaware of it. `Participants` injection is only to externalize its life-cycle management. This way it can be created upon host startup single time and on each `Chat` creation for unit-tests. Since the current implementation is constrained to single server deployment, this extension point can also be used in future to enable multi-server installations (e.g. by storing participants in database or external cache).

## Automated tests

### End-to-end through the browser
They use [https://github.com/featurist/coypu](Coypu) with [https://github.com/SeleniumHQ/selenium](Selenium) underneath to instrument browser of choice (Chrome for development and PhantomJS for CI by default, which is controlled by CI system environment variable). Those tests are rather slow, but allow to tests full interaction including rendering phase (so for example one can and we do check for XSS attacks with it easily). Since for the purpose of those tests to be runnable on CI server, IISExpress has been included. Before any test executes it loads up Web host application and starts it up. Since RavenDB used normally, takes a while to create it structures on the first load, it is mocked by volatile `InMemoryMessageLog` implementation for the purposes of those tests (this is controlled via UnderTest system environment variable). Except from that

### Unit/Interaction through `ChatHub` 
All network interaction has been doubled, so they do not cross process at any moment. However they still test `ChatHub` and underlying domain layer classes like `Chat` together, so those are not per-definition unit tests. Regardless of that, in contrast to end-to-end ones, they are incredibly fast (milliseconds  to seconds/minutes) and hence allow for much greater granularity. They by default use `InMemoryMessageLog`, so they are independant of each other. Assertions/checks are evaluated based on  set of spies implementing types interface `ClientOperationsSpy`, which allows to assert on messages delivery to clients (without involving actual network calls)
