# NAGP 2026 - Kubernetes Assignment

## Employee API - Multi-Tier Kubernetes Deployment

A containerized .NET 9 Web API deployed on Kubernetes (GKE) with PostgreSQL database, demonstrating cloud-native best practices including auto-scaling, self-healing, rolling updates, and FinOps resource optimization.

---

## Quick Links

| Resource | URL |
|----------|-----|
| **Code Repository** | https://github.com/saurabhjain374/nagp-k8s-assignment |
| **Docker Hub Image** | https://hub.docker.com/r/saurabhjain374/employee-api |
| **API Endpoint (Ingress)** | http://8.231.113.109/api/employees |
| **API Endpoint (LoadBalancer)** | http://34.180.14.51/api/employees |
| **Swagger UI** | http://34.180.14.51/swagger |

---

## Project Structure

```
nagp-k8s-assignment/
├── nagp-k8s-assignment-app/          # Application source code
│   └── EmployeeApi/
│       ├── Controllers/              # API Controllers
│       ├── Models/                   # Data models
│       ├── Repositories/             # Database access layer
│       ├── Dockerfile                # Multi-stage Docker build
│       └── Program.cs                # Application entry point
│
├── nagp-k8s-assignment-k8s/          # Kubernetes manifests
│   ├── namespace.yaml                # Namespace definition
│   ├── employee-api-deployment.yaml  # API Deployment with HPA
│   ├── employee-api-service.yaml     # LoadBalancer Service
│   ├── api-configmap.yaml            # API configuration
│   ├── postgres-deployment.yaml      # Database Deployment
│   ├── postgres-service.yaml         # ClusterIP Service
│   ├── postgres-configmap.yaml       # Database configuration
│   ├── postgres-secret.yaml          # Database credentials
│   ├── postgres-pvc.yaml             # Persistent Volume Claim
│   ├── hpa.yaml                      # Horizontal Pod Autoscaler
│   └── ingress.yaml                  # NGINX Ingress
│
└── docs/                             # Documentation
    └── DOCUMENTATION.md              # Comprehensive documentation
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/employees` | Get all employees |
| GET | `/api/employees/{id}` | Get employee by ID |
| POST | `/api/employees` | Create new employee |
| GET | `/api/employees/info` | Get application info (environment, version, hostname) |
| GET | `/health` | Health check endpoint |
| GET | `/swagger` | Swagger UI |

---

## Architecture Overview

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

---

## Deployment Instructions

### Prerequisites
- Kubernetes cluster (GKE, AKS, EKS, or local)
- kubectl configured
- NGINX Ingress Controller installed

### Deploy to Kubernetes

```bash
# 1. Create namespace
kubectl apply -f nagp-k8s-assignment-k8s/namespace.yaml

# 2. Deploy database tier
kubectl apply -f nagp-k8s-assignment-k8s/postgres-secret.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-configmap.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-pvc.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-deployment.yaml
kubectl apply -f nagp-k8s-assignment-k8s/postgres-service.yaml

# 3. Deploy API tier
kubectl apply -f nagp-k8s-assignment-k8s/api-configmap.yaml
kubectl apply -f nagp-k8s-assignment-k8s/employee-api-deployment.yaml
kubectl apply -f nagp-k8s-assignment-k8s/employee-api-service.yaml
kubectl apply -f nagp-k8s-assignment-k8s/hpa.yaml
kubectl apply -f nagp-k8s-assignment-k8s/ingress.yaml

# 4. Verify deployment
kubectl get all -n nagp-assignment
```

---

## Kubernetes Features Demonstrated

| Feature | Implementation |
|---------|----------------|
| **Rolling Updates** | `strategy: type: RollingUpdate` in API deployment |
| **Self-Healing** | Liveness & Readiness probes on `/health` endpoint |
| **Auto-Scaling (HPA)** | CPU-based scaling (70% threshold, 2-10 replicas) |
| **Data Persistence** | PersistentVolumeClaim for PostgreSQL (5Gi) |
| **Configuration Management** | ConfigMaps for non-sensitive config |
| **Secrets Management** | Kubernetes Secrets for DB credentials (base64 encoded) |
| **Service Discovery** | ClusterIP service for internal DB access |
| **External Access** | LoadBalancer + NGINX Ingress |
| **Resource Management** | CPU/Memory requests and limits defined |

---

## FinOps Considerations

Resource optimization implemented:

| Resource | Request | Limit |
|----------|---------|-------|
| CPU | 50m | 500m |
| Memory | 256Mi | 512Mi |

See [DOCUMENTATION.md](docs/DOCUMENTATION.md) for detailed FinOps analysis and cost optimization opportunities.

---

## Screen Recording Demonstrations

The video recording demonstrates:
1. ✅ All Kubernetes objects deployed and running
2. ✅ API call retrieving employee records from database
3. ✅ Self-healing: API pod deletion and automatic regeneration
4. ✅ Database persistence: Pod deletion with data retention
5. ✅ HPA in action with resource metrics
6. ✅ Rolling update deployment strategy

---

## Technology Stack

- **Application:** .NET 9 Web API
- **Database:** PostgreSQL 17
- **Container Runtime:** Docker
- **Orchestration:** Kubernetes (GKE)
- **Ingress Controller:** NGINX
- **ORM:** Dapper (micro-ORM)

---

## Author

**Saurabh Jain**  
NAGP 2026 - Technology Band III  
Kubernetes, DevOps & FinOps Workshop

