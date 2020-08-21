
echo $resourceGroup
echo $location
echo $clusterName
echo $spId
echo $secret

az group create --name $resourceGroup --location $location

# az aks create --resource-group $resourceGroup --name $clusterName --generate-ssh-keys --service-principal $spId --client-secret $secret

# az aks use-dev-spaces -g $resourceGroup -n $clusterName