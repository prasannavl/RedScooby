// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Api.Core
{
    public class RedScoobyApiException : Exception { }

    public class ApiKeyNotAuthorizedException : RedScoobyApiException { }

    public class UserNotAuthenticatedException : RedScoobyApiException { }
}
