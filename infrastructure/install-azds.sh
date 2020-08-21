

echo $resourceGroup
echo $location
echo $clusterName
echo $spId
echo $secret

az aks use-dev-spaces -g $resourceGroup -n $clusterName -s team-space -y
