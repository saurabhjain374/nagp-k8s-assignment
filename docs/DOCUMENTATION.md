# NAGP 2026 Kubernetes Assignment

## What this assignment is about

Basically we need to make a kubernetes app that has two parts:

- An API that people can access from outside. It should scale up/down on its own and restart if something goes wrong. Need 4 pods, rolling updates and HPA stuff
- A database (PostgreSQL) that only the API can talk to. This one needs storage that stays even if pod dies
- Also need to use ConfigMaps and Secrets for config and passwords
- Should think about costs too (FinOps part)

## Things I assumed

- Using GKE (Google cloud kubernetes) with NGINX for ingress
- Storage class standard-rwo is there by default
- Not expecting heavy traffic, just normal usage
- Metrics server is running so HPA can work

## My Solution

### What I used

- API is made with .NET 9
- Database is PostgreSQL 17
- Using Dapper for database queries (its simpler than EF)
- Everything runs on Kubernetes in GKE
- NGINX handles incoming traffic

### How it works

Basically traffic comes from internet, goes to ingress, then to API pods (these scale between 2 to 10 based on load), then API talks to postgres through ClusterIP service, and postgres saves data to persistent volume.

### K8s files I created

- employee-api deployment - this is the API, has health checks and rolling update
- postgres deployment - database pod with PVC attached
- employee-api-service - LoadBalancer type so external access works
- postgres-service - ClusterIP so only internal access
- api-config and postgres-config - ConfigMaps for settings
- postgres-secret - has the database password
- postgres-pvc - 5Gi storage for database
- employee-api-hpa - autoscaling based on CPU
- ingress - routes traffic to API

### Main features

- Rolling updates so no downtime when deploying
- Health checks at /health endpoint, pods restart if they fail
- HPA kicks in at 70% CPU, keeps between 2 and 10 pods
- Data stays safe in PVC even if postgres pod restarts

## Why I chose these resource values

| What           | Value | Why                                                          |
| -------------- | ----- | ------------------------------------------------------------ |
| CPU Request    | 50m   | .NET doesnt use much when idle, around 20-30m so 50m is fine |
| CPU Limit      | 500m  | Gives room to handle sudden load                             |
| Memory Request | 256Mi | .NET needs some memory for runtime                           |
| Memory Limit   | 512Mi | Double the request, should be enough                         |
| Storage        | 5Gi   | More than enough for this demo                               |
| API pods       | 2-10  | 2 minimum for availability, 10 max so we dont spend too much |
| DB pods        | 1     | Just one is okay for demo purpose                            |

## FinOps - How to save money

### What I already did

- Kept resource requests low (50m CPU, 256Mi memory)
- HPA starts with just 4 pods
- Set limits so pods dont eat up all resources

### More things we can do to save money

**1. Spot instances**
Run API pods on spot/preemptible nodes. These are way cheaper (like 60-90% off). If they get killed its okay because we have multiple pods and HPA.

**2. Cluster autoscaler**
Let the cluster add/remove nodes based on how many pods need to run. Saves money at night or weekends when nobody is using the app. Should also add PodDisruptionBudget so atleast some pods are always there.

**3. Smaller docker images**
Right now image is around 220MB. Can use alpine based images and get it down to like 50MB. Smaller image means faster deployments and less storage cost.

---

Saurabh Jain - NAGP 2026
