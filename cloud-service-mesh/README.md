# Cloud Service Mesh

## Installation

### Onboarding

- https://cloud.google.com/service-mesh/docs/onboarding/provision-control-plane

```
gcloud auth login --project $PROJECT_ID
gcloud services enable mesh.googleapis.com \
      --project=$PROJECT_ID
echo "management: automatic" > mesh.yaml
gcloud container fleet mesh enable --project $PROJECT_ID \
    --fleet-default-member-config mesh.yaml

gcloud container clusters update $NAME \
  --location $ZONE \
  --fleet-project $PROJECT_ID
gcloud container fleet memberships list --project $PROJECT_ID
```

### Certificate Authority-service

- https://cloud.google.com/service-mesh/docs/security/certificate-authority-service
- Steps
  - Create Root CA pool
  - Create Root CA
  - Create Subordinate CA pool
  - Create Subordinate CA
  - Update policy and permission
  - Update config in Kuberentes

### Mesh automatic management

```
  gcloud container fleet mesh update \
     --management automatic \
     --memberships ${NAME} \
     --project $PROJECT_ID \
     --location $LOCATION
```

### Enable Cloud Trace

- https://cloud.google.com/service-mesh/docs/observability/accessing-traces
- istio Telemetry API https://istio.io/latest/docs/tasks/observability/telemetry/

## Demo Application

### Application Setup

#### Ingress Gateway

```
kubectl create namespace asm-ingress
kubectl label namespace asm-ingress \
      istio.io/rev- istio-injection=enabled --overwrite
kubectl apply -n asm-ingress \
-f service-mesh/asm-ingress-gateway
```

#### Online Boutique 
```
kubectl create namespace onlineboutique
kubectl label namespace onlineboutique \
      istio.io/rev- istio-injection=enabled --overwrite

kubectl apply \
-n onlineboutique \
-f service-mesh/online-boutique/virtual-service.yaml

kubectl apply \
-n onlineboutique \
-f service-mesh/online-boutique/service-accounts

kubectl get services -n asm-ingress

export FRONTEND_IP=$(kubectl --namespace asm-ingress \
get service --output jsonpath='{.items[0].status.loadBalancer.ingress[0].ip}' \
)

for i in {0..10}; do
curl -s -I $FRONTEND_IP ; done
```

### Authorization

- https://cloud.google.com/service-mesh/docs/tutorials/authz#managed-istiod

```
kubectl -n onlineboutique apply -f service-mesh/authorization/currency-deny-all.yaml

CURRENCY_POD=$(kubectl get pod -n onlineboutique |grep currency|awk '{print $1}') 

kubectl  -n onlineboutique apply -f service-mesh/authorization/currency-allow-frontend-checkout.yaml
```

### Canary Deployment

- https://cloud.google.com/service-mesh/docs/tutorials/canary-deployment

```
kubectl -n onlineboutique patch deployments/productcatalogservice -p '{"spec":{"template":{"metadata":{"labels":{"version":"v1"}}}}}' 

kubectl  -n onlineboutique apply -f service-mesh/canary-service/destination-vs-v1.yaml
kubectl  -n onlineboutique apply -f service-mesh/canary-service/productcatalog-v2.yaml
kubectl  -n onlineboutique apply -f service-mesh/canary-service/destination-v1-v2.yaml
kubectl  -n onlineboutique apply -f service-mesh/canary-service/vs-split-traffic.yaml

export FRONTEND_IP=$(kubectl --namespace asm-ingress \
get service --output jsonpath='{.items[0].status.loadBalancer.ingress[0].ip}' \
)

for i in {0..100}; do
curl -s -I $FRONTEND_IP ; done
```

### Service Mesh + Cloud Trace

```
kubectl apply -f test-pod/test-curl.yaml
kubectl -n onlineboutique apply -f test-pod/httpbin.yaml
kubectl exec -it testcurl -- curl "httpbin.onlineboutique.svc.cluster.local:8000/anything?freeform=" -H "accept: application/json" -H "Traceparent: 00-7543d15e09e5d61801d4f74cde1269b8-604ef051d35c5b3f-01" -vv
```