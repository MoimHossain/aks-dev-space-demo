
# Demo AKS Dev Space

A simple demonstration of AKS DevSpace

![Diagram](https://docs.microsoft.com/en-us/azure/dev-spaces/media/how-dev-spaces-works/prepare-cluster.svg)

# Demo Setup Instructions

Follow the instrcution to setup the demo.

## Creating cluster

```
 > az group create --name moim-dev-space --location westeurope
```
```
 > az aks create --resource-group moim-dev-space --name moimha-aks-devspace --generate-ssh-keys --service-principal <SP ID> --client-secret <SECRET>
```
```
 > az aks get-credentials -g moim-dev-space -n moimha-aks-devspace --overwrite-existing
```

Once the cluster is created we will enable the AKS Dev Space in it with following command:

```
> az aks use-dev-spaces -g moim-dev-space -n moimha-aks-devspace
```

Once installed, if you want to check the ```azds``` namespace in AKS, inspect the following
```
 > kubectl get all -n azds
```

## Creating application

Start by creating a ```team``` dev space - that's stable for everybody in the team.

```
 > azds space select --name team
```

### Microservices

Let's now create some microservices into the space:

#### Backend

- Create a directory ```backend```
- Run ``` > dotnet new web```
- Modify the code in ```Startup.cs``` with below:

```csharp
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Hello World from Backend!");
        });
    });
```

- Run ``` > azds prep --enable-ingress```
- Run ``` > azds up -d ```

Now the backend is running in the AKS (in ```team``` space)

#### API

- Create a directory ```api```
- Run ``` > dotnet new web```
- Modify the code in ```Startup.cs``` with below:

```csharp
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
                using var client = new System.Net.Http.HttpClient();
                var request = new System.Net.Http.HttpRequestMessage();
                request.RequestUri = new Uri("http://backend/");
                var header = "azds-route-as";
                if (context.Request.Headers.ContainsKey(header))
                {
                    request.Headers.Add(header, context.Request.Headers[header] as IEnumerable<string>);
                }
                var response = await client.SendAsync(request);
                if(response.IsSuccessStatusCode) {
                    await context.Response.WriteAsync($"API saw this --> {await response.Content.ReadAsStringAsync()}");
                }
                else {
                    await context.Response.WriteAsync("Couldn't connect...");
                } 
        });
    });
```
- Run ``` > azds prep --enable-ingress```
- Run ``` > azds up -d ```

Now the API is running! Finally,

#### Frontend

- Create a directory ```frontend```
- Run ``` > dotnet new web```
- Modify the code in ```Startup.cs``` with below:

```csharp
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            using var client = new System.Net.Http.HttpClient();
            var request = new System.Net.Http.HttpRequestMessage();
            request.RequestUri = new Uri("http://api/");
            var header = "azds-route-as";
            if (context.Request.Headers.ContainsKey(header))
            {
                request.Headers.Add(header, context.Request.Headers[header] as IEnumerable<string>);
            }
            var response = await client.SendAsync(request);
            if(response.IsSuccessStatusCode) {
                await context.Response.WriteAsync($"Front End Saw--> {await response.Content.ReadAsStringAsync()}");
            }
            else {
                await context.Response.WriteAsync("Couldn't connect...");
            }  
        });
    });
```

- Run ``` > azds prep --enable-ingress```
- Run ``` > azds up -d ```

Great, now you should have your app running in stable space named **team**. And you should have an URL to your frontend similar to the following url:

> http://team.frontend.l97bxx42cf.weu.azds.io/

### Create dev space in isolation 

We will do that for the **api** project. Navigate to the api directory from WSL.

- Create a new space for the Developer (i.e. myself)
- Run ``` > azds space select moim-personal```
- It will prompt to select a space as parent, select ```team``` as parent.
- Change the ```Startup.cs``` of API project - modify the message. (i.e. with V2 or something...)
- Run ``` > azds up -d```
- You should have a new URL now for your space, similar to below URL:

> http://moimha.s.team.frontend.l97bxx42cf.weu.azds.io/

Notice the ```moimha.s.``` came before the ```team.frontend.*``` into the URL- that's how the url hierarchy is built.

You should now be able to see both versions by navigating the corresponding urls.

## That's it.