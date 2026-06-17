# NAGP 2026 Kubernetes Assignment - Documentation

## 1. Requirement Understanding

Design and deploy a multi-tier Kubernetes application with:
- **Service API Tier**: Externally accessible, auto-scaling, self-healing API (4 pods, rolling updates, HPA)
- **Database Tier**: Internal-only PostgreSQL with persistent storage (1 pod)
- **Configuration**: ConfigMaps for settings, Secrets for credentials
- **FinOps**: Resource limits and cost optimization

---

## 2. Assumptions

- **Platform**: Google Kubernetes Engine (GKE) with NGINX Ingress Controller
- **Storage**: Default `standard-rwo` storage class available
- **Traffic**: Low-moderate traffic; HPA handles bursts
- **Monitoring**: Kubernetes metrics server enabled for HPA

---

## 3. Solution Overview

### Technology Stack
| Component | Technology |
|-----------|------------|
| API | .NET 9 Web API |
| Database | PostgreSQL 17 |
| ORM | Dapper |
| Orchestration | Kubernetes (GKE) |
| Ingress | NGINX |

### Architecture
```
Internet → Ingress/LoadBalancer → API Pods (HPA: 2-10) → ClusterIP → PostgreSQL Pod → PVC (5Gi)
```

### Kubernetes Resources
| Resource | Name | Purpose |
|----------|------|---------|
| Deployment | employee-api | API with rolling updates, probes |
| Deployment | postgres | Database with PVC |
| Service | employee-api-service | LoadBalancer (external) |
| Service | postgres-service | ClusterIP (internal) |
| ConfigMap | api-config, postgres-config | Configuration |
| Secret | postgres-secret | DB credentials |
| PVC | postgres-pvc | 5Gi persistent storage |
| HPA | employee-api-hpa | CPU-based scaling (70%) |
| Ingress | employee-api-ingress | External routing |

### Key Features
- **Rolling Updates**: Zero-downtime deployments
- **Self-Healing**: Liveness/Readiness probes on `/health`
- **HPA**: CPU 70% threshold, 2-10 replicas
- **Persistence**: PVC survives pod restarts

---

## 4. Justification for Resources

| Resource | Value | Justification |
|----------|-------|---------------|
| CPU Request | 50m | Minimal baseline; .NET idles at ~20-30m |
| CPU Limit | 500m | 10x burst for request spikes |
| Memory Request | 256Mi | Covers .NET runtime overhead |
| Memory Limit | 512Mi | 2x buffer prevents OOM |
| Storage | 5Gi | Adequate for sample data |
| API Replicas | 2-10 | Min 2 for HA, max 10 for cost control |
| DB Replicas | 1 | Single pod sufficient for demo |

---

## 5. FinOps - Cost Optimization Opportunities

### Implemented
- Right-sized resource requests (CPU: 50m, Memory: 256Mi)
- HPA with low minimum replicas (2)
- Appropriate limits prevent resource hogging

### Three Optimization Opportunities

**1. Use Spot/Preemptible Instances**
- Deploy API tier on spot nodes
- **Savings**: 60-90% compute costs
- Risk mitigated by HPA and self-healing

**2. Enable Cluster Autoscaler**
- Auto-scale nodes based on demand
- Add PodDisruptionBudget for availability
- **Savings**: 20-40% during low-traffic periods

**3. Optimize Container Images**
- Use Alpine-based or distroless images
- Reduces image size from ~220MB to ~50MB
- **Savings**: Faster startup, lower storage/transfer costs

---

*Author: Saurabh Jain | NAGP 2026*
