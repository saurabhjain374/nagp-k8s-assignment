# NAGP 2026 - Kubernetes Assignment

## Employee API on Kubernetes

This is my kubernetes assignment where I made a .NET 9 API with PostgreSQL database. Deployed it on GKE with autoscaling, health checks, rolling updates and all that stuff.

---

## Links

| What | Link |
|------|------|
| Github Repo | https://github.com/saurabhjain374/nagp-k8s-assignment |
| Docker Image | https://hub.docker.com/r/saurabhjain374/employee-api |
| API via Ingress | http://8.231.113.109/api/employees |
| API via LoadBalancer | http://34.180.14.51/api/employees |
| Swagger | http://34.180.14.51/swagger |

---

## Folder Structure

```
nagp-k8s-assignment/
├── nagp-k8s-assignment-app/          # the actual code
│   └── EmployeeApi/
│       ├── Controllers/              # api controllers
│       ├── Models/                   # employee model etc
│       ├── Repositories/             # database stuff
│       ├── Dockerfile                # for building image
│       └── Program.cs                # main file
│
├── nagp-k8s-assignment-k8s/          # kubernetes yaml files
│   ├── namespace.yaml
│   ├── employee-api-deployment.yaml  # api deployment
│   ├── employee-api-service.yaml     # loadbalancer service
│   ├── api-configmap.yaml
│   ├── postgres-deployment.yaml      # db deployment
│   ├── postgres-service.yaml         # clusterip service
│   ├── postgres-configmap.yaml
│   ├── postgres-secret.yaml          # db password
│   ├── postgres-pvc.yaml             # storage
│   ├── hpa.yaml                      # autoscaling
│   └── ingress.yaml
│
└── docs/
    └── DOCUMENTATION.md              # detailed docs
```

---

## API Endpoints

| Method | URL | What it does |
|--------|-----|--------------|
| GET | `/api/employees` | gets all employees |
| GET | `/api/employees/{id}` | gets one employee |
| POST | `/api/employees` | creates new employee |
| GET | `/api/employees/info` | shows app info like hostname |
| GET | `/health` | health check |
| GET | `/swagger` | swagger ui |

---

## How it works

```
                    ┌─────────────────────────────────────────────────────┐
                    │                   KUBERNETES CLUSTER                │
                    │                                                     │
    Internet        │  ┌─────────────┐    ┌─────────────────────────┐    │
        │           │  │   Ingress   │    │    Service API Tier     │    │
        │           │  │   (NGINX)   │───▶│  ┌─────┐ ┌─────┐       │    │
        ▼           │  └─────────────┘    │  │ Pod │ │ Pod │  ...  │    │
   ┌─────────┐      │                     │  └──┬──┘ └──┬──┘       │    │
   │  Users  │──────┼──────────────────────────▶│       │          │    │
   └─────────┘      │  LoadBalancer       │     └───────┴──────────┤    │
                    │                     │            │           │    │
                    │                     │            ▼           │    │
                    │                     │    ┌──────────────┐    │    │
                    │                     │    │ ClusterIP    │    │    │
                    │                     │    │   Service    │    │    │
                    │                     │    └──────┬───────┘    │    │
                    │                     │           │            │    │
                    │                     │           ▼            │    │
                    │                     │  ┌────────────────┐    │    │
                    │                     │  │  Database Tier │    │    │
                    │                     │  │  ┌──────────┐  │    │    │
                    │                     │  │  │ Postgres │  │    │    │
                    │                     │  │  │   Pod    │  │    │    │
                    │                     │  │  └────┬─────┘  │    │    │
                    │                     │  │       │        │    │    │
                    │                     │  │  ┌────▼─────┐  │    │    │
                    │                     │  │  │   PVC    │  │    │    │
                    │                     │  │  │  (5 Gi)  │  │    │    │
                    │                     │  │  └──────────┘  │    │    │
                    │                     │  └────────────────┘    │    │
                    │                     └────────────────────────┘    │
                    └─────────────────────────────────────────────────────┘
```

Basically users hit the ingress or loadbalancer, that goes to API pods (which can scale from 2 to 10), API talks to postgres through clusterip, and postgres saves to PVC.

---

## How to deploy

### You need
- A kubernetes cluster (I used GKE but any should work)
- kubectl setup
- NGINX ingress controller

### Commands

```bash
# create namespace first
kubectl apply -f nagp-k8s-assignment-k8s/namespace.yaml

# setup database
kubectl apply -f nagp-k8s-assignment-k8s/postgres-secret.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-configmap.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-pvc.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-deployment.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-service.yaml

# setup api
kubectl apply -f nagp-k8s-assignment-k8s/api-configmap.yaml
kubectl apply -f nagp-k8s-assignment-k8s/employee-api-deployment.yaml
kubectl apply -f nagp-k8s-assignment-k8s/employee-api-service.yaml
kubectl apply -f nagp-k8s-assignment-k8s/hpa.yaml
kubectl apply -f nagp-k8s-assignment-k8s/ingress.yaml

# check if everything is running
kubectl get all -n nagp-assignment
```

---

## K8s features I used

| Feature | How I did it |
|---------|--------------|
| Rolling Updates | set strategy type to RollingUpdate in deployment |
| Self Healing | added liveness and readiness probes hitting /health |
| Autoscaling | HPA with 70% cpu target, min 2 max 10 pods |
| Persistent Data | PVC for postgres, 5Gi storage |
| Config | ConfigMaps for settings |
| Secrets | K8s secrets for db password (base64) |
| Internal access | ClusterIP for postgres so only api can reach it |
| External access | LoadBalancer + ingress |
| Resource limits | set cpu and memory requests/limits |

---

## Resource values

| Resource | Request | Limit |
|----------|---------|-------|
| CPU | 50m | 500m |
| Memory | 256Mi | 512Mi |

Check [DOCUMENTATION.md](docs/DOCUMENTATION.md) for more details on why I chose these values and cost saving ideas.

---

## Video Recording

In the video I showed:
1. All pods running properly
2. API working and fetching data from db
3. Deleted api pod and it came back on its own (self healing)
4. Deleted postgres pod and data was still there (persistence)
5. HPA showing metrics
6. Rolling update happening

---

## Tech used

- .NET 9 for API
- PostgreSQL 17 for database
- Docker for containers
- Kubernetes on GKE
- NGINX for ingress
- Dapper for database queries

---

Saurabh Jain  
NAGP 2026

