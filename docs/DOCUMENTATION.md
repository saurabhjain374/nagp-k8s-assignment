# NAGP 2026 Kubernetes Assignment - Comprehensive Documentation

## Table of Contents
1. [Requirement Understanding](#1-requirement-understanding)
2. [Assumptions](#2-assumptions)
3. [Solution Overview](#3-solution-overview)
4. [Justification for Resources Utilized](#4-justification-for-resources-utilized)
5. [FinOps - Cost Optimization Opportunities](#5-finops---cost-optimization-opportunities)

---

## 1. Requirement Understanding

### 1.1 Overview
The assignment requires designing, containerizing, and deploying a multi-tier architecture on Kubernetes consisting of:
- **Service API Tier**: A microservice exposing REST APIs that fetch data from a database
- **Database Tier**: A persistent database with pre-populated sample data

### 1.2 Service API Tier Requirements

| Requirement | Understanding |
|-------------|---------------|
| API Exposure | Service must expose HTTP endpoints accessible from outside the cluster |
| Database Integration | Must connect to and fetch data from the database tier |
| Technology Stack | Any standard language/framework (.NET, Java, Node.js, Python) |
| Best Practices | Connection pooling, configuration separation from code |
| Rolling Updates | Support zero-downtime deployments |
| External Access | Must be accessible from outside the Kubernetes cluster |
| Self-Healing | Automatically recover from failures |
| Auto-Scaling | HPA based on resource utilization |
| Replicas | 4 pods (or HPA-managed scaling) |
| Configuration | Use ConfigMaps for external configuration |
| Secrets | Use Kubernetes Secrets for sensitive data |

### 1.3 Database Tier Requirements

| Requirement | Understanding |
|-------------|---------------|
| Data | One table with 5-10 sample records |
| Persistence | Data must survive pod restarts/deletions |
| Access | Internal cluster access only (no external exposure) |
| Recovery | Automatic pod recreation after failure |
| Replicas | 1 pod |
| Secrets | Use Kubernetes Secrets for credentials |

### 1.4 Kubernetes Requirements

| Requirement | Understanding |
|-------------|---------------|
| ConfigMap | Database configuration externalized via ConfigMap |
| Secrets | Passwords stored as base64-encoded Secrets |
| Persistence | PersistentVolumeClaim for database storage |
| Service Discovery | Use Kubernetes Services (not pod IPs) for inter-tier communication |
| Ingress | External API access through Ingress controller |

### 1.5 FinOps Requirements

| Requirement | Understanding |
|-------------|---------------|
| Resource Limits | Define CPU and memory requests/limits |
| Cost Optimization | Identify 3+ opportunities to reduce costs |
| Metrics-Based Optimization | Implement resource tuning based on observed metrics |

---

## 2. Assumptions

### 2.1 Infrastructure Assumptions
1. **Cloud Provider**: Google Kubernetes Engine (GKE) is used as the Kubernetes platform
2. **Ingress Controller**: NGINX Ingress Controller is pre-installed in the cluster
3. **Storage Class**: Default `standard-rwo` storage class is available for persistent volumes
4. **Networking**: Standard Kubernetes networking with LoadBalancer support

### 2.2 Application Assumptions
1. **Traffic Pattern**: Low to moderate traffic expected; HPA configured for burst handling
2. **Data Volume**: Small dataset (5-10 employee records); minimal storage requirements
3. **Security**: Basic authentication not implemented; focus is on Kubernetes features
4. **Environment**: Single environment (production) deployment

### 2.3 Operational Assumptions
1. **Monitoring**: Kubernetes metrics server is available for HPA functionality
2. **Cluster Resources**: Sufficient cluster capacity for all pods
3. **Network Policies**: No strict network policies; default cluster networking
4. **DNS**: Kubernetes internal DNS is functional for service discovery

### 2.4 Cost Assumptions
1. **Cluster Type**: Autopilot or Standard GKE cluster
2. **Region**: Single region deployment
3. **Resource Optimization**: Right-sizing based on actual usage patterns

---

## 3. Solution Overview

### 3.1 Technology Choices

| Component | Technology | Justification |
|-----------|------------|---------------|
| **API Framework** | .NET 9 Web API | Modern, high-performance, cross-platform framework |
| **Database** | PostgreSQL 17 | Robust, open-source RDBMS with excellent Kubernetes support |
| **ORM** | Dapper | Lightweight micro-ORM for efficient database access |
| **Container Runtime** | Docker | Industry standard for containerization |
| **Orchestration** | Kubernetes (GKE) | Enterprise-grade container orchestration |
| **Ingress** | NGINX | Popular, well-supported ingress controller |

### 3.2 Architecture Design

```
┌──────────────────────────────────────────────────────────────────────────┐
│                         KUBERNETES CLUSTER (GKE)                         │
│                         Namespace: nagp-assignment                       │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                      SERVICE API TIER                            │    │
│  │                                                                  │    │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │    │
│  │  │ employee-api │  │ employee-api │  │ employee-api │  (HPA)    │    │
│  │  │    Pod 1     │  │    Pod 2     │  │    Pod N     │  2-10     │    │
│  │  │              │  │              │  │              │  replicas │    │
│  │  │  ┌────────┐  │  │  ┌────────┐  │  │  ┌────────┐  │           │    │
│  │  │  │ .NET 9 │  │  │  │ .NET 9 │  │  │  │ .NET 9 │  │           │    │
│  │  │  │  API   │  │  │  │  API   │  │  │  │  API   │  │           │    │
│  │  │  └────────┘  │  │  └────────┘  │  │  └────────┘  │           │    │
│  │  └──────────────┘  └──────────────┘  └──────────────┘           │    │
│  │         │                 │                 │                    │    │
│  │         └─────────────────┼─────────────────┘                    │    │
│  │                           ▼                                      │    │
│  │              ┌─────────────────────────┐                         │    │
│  │              │  employee-api-service   │                         │    │
│  │              │     (LoadBalancer)      │◄──── External Access    │    │
│  │              │    Port 80 → 8080       │                         │    │
│  │              └─────────────────────────┘                         │    │
│  │                           │                                      │    │
│  │              ┌────────────┴────────────┐                         │    │
│  │              │   employee-api-ingress  │◄──── NGINX Ingress      │    │
│  │              │     (nginx class)       │                         │    │
│  │              └─────────────────────────┘                         │    │
│  │                                                                  │    │
│  │  Configuration:                                                  │    │
│  │  ├── api-config (ConfigMap): APP_ENVIRONMENT, APP_VERSION       │    │
│  │  ├── postgres-config (ConfigMap): DB_HOST, DB_PORT, DB_NAME     │    │
│  │  └── postgres-secret (Secret): DB_USER, DB_PASSWORD             │    │
│  │                                                                  │    │
│  └──────────────────────────────────────────────────────────────────┘    │
│                                    │                                     │
│                                    ▼                                     │
│  ┌──────────────────────────────────────────────────────────────────┐    │
│  │                      DATABASE TIER                                │    │
│  │                                                                   │    │
│  │  ┌──────────────────────┐      ┌─────────────────────────┐       │    │
│  │  │    postgres Pod      │      │    postgres-service     │       │    │
│  │  │                      │◄─────│      (ClusterIP)        │       │    │
│  │  │  ┌────────────────┐  │      │    Port 5432            │       │    │
│  │  │  │  PostgreSQL 17 │  │      │   Internal Only         │       │    │
│  │  │  │                │  │      └─────────────────────────┘       │    │
│  │  │  │  ┌──────────┐  │  │                                        │    │
│  │  │  │  │employees │  │  │      Configuration:                    │    │
│  │  │  │  │  table   │  │  │      ├── postgres-config (ConfigMap)   │    │
│  │  │  │  │ 10 rows  │  │  │      └── postgres-secret (Secret)      │    │
│  │  │  │  └──────────┘  │  │                                        │    │
│  │  │  └────────────────┘  │                                        │    │
│  │  │          │           │                                        │    │
│  │  │          ▼           │                                        │    │
│  │  │  ┌────────────────┐  │                                        │    │
│  │  │  │  Volume Mount  │  │                                        │    │
│  │  │  └────────────────┘  │                                        │    │
│  │  └──────────┬───────────┘                                        │    │
│  │             │                                                    │    │
│  │             ▼                                                    │    │
│  │  ┌──────────────────────┐                                        │    │
│  │  │    postgres-pvc      │                                        │    │
│  │  │  PersistentVolume    │                                        │    │
│  │  │       5 Gi           │                                        │    │
│  │  │  (standard-rwo)      │                                        │    │
│  │  └──────────────────────┘                                        │    │
│  │                                                                   │    │
│  └───────────────────────────────────────────────────────────────────┘    │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

### 3.3 Kubernetes Resources Summary

| Resource Type | Name | Purpose |
|---------------|------|---------|
| Namespace | nagp-assignment | Logical isolation for all resources |
| Deployment | employee-api | API tier with rolling updates, probes |
| Deployment | postgres | Database tier with persistent storage |
| Service | employee-api-service | LoadBalancer for external API access |
| Service | postgres-service | ClusterIP for internal DB access |
| ConfigMap | api-config | API environment configuration |
| ConfigMap | postgres-config | Database connection configuration |
| Secret | postgres-secret | Database credentials |
| PVC | postgres-pvc | 5Gi persistent storage for database |
| HPA | employee-api-hpa | Auto-scaling based on CPU (2-10 pods) |
| Ingress | employee-api-ingress | NGINX ingress for external routing |

### 3.4 Key Implementation Details

#### 3.4.1 Rolling Updates Strategy
```yaml
strategy:
  type: RollingUpdate
```
- Ensures zero-downtime deployments
- Gradually replaces old pods with new ones
- Maintains service availability during updates

#### 3.4.2 Self-Healing with Probes
```yaml
livenessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 15
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 10
  failureThreshold: 3
```
- **Liveness Probe**: Restarts unhealthy containers
- **Readiness Probe**: Removes unready pods from service endpoints
- **Health Endpoint**: Custom `/health` endpoint in API

#### 3.4.3 Horizontal Pod Autoscaler
```yaml
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: employee-api
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```
- Scales based on CPU utilization (70% threshold)
- Minimum 2 replicas for high availability
- Maximum 10 replicas for burst handling

#### 3.4.4 Configuration Separation
- **ConfigMaps**: Non-sensitive configuration (DB host, port, environment)
- **Secrets**: Sensitive data (DB username, password) - base64 encoded
- **Environment Variables**: Injected into containers at runtime

#### 3.4.5 Data Persistence
```yaml
volumes:
- name: postgres-storage
  persistentVolumeClaim:
    claimName: postgres-pvc
```
- PVC with 5Gi storage
- Data survives pod restarts and deletions
- Uses `standard-rwo` storage class (GKE default)

---

## 4. Justification for Resources Utilized

### 4.1 CPU Resources

| Component | Request | Limit | Justification |
|-----------|---------|-------|---------------|
| employee-api | 50m | 500m | Low baseline (50m) for idle state; 500m limit allows burst handling for API requests |
| postgres | Default | Default | Database uses cluster defaults; single pod doesn't require strict limits |

**Rationale:**
- **50m request**: Minimal CPU guaranteed; typical .NET API idles at ~20-30m
- **500m limit**: 10x burst capacity handles concurrent request spikes
- **Cost-effective**: Low requests reduce cluster resource consumption

### 4.2 Memory Resources

| Component | Request | Limit | Justification |
|-----------|---------|-------|---------------|
| employee-api | 256Mi | 512Mi | .NET runtime requires ~200Mi; 512Mi limit prevents OOM under load |
| postgres | Default | Default | PostgreSQL manages memory efficiently with defaults |

**Rationale:**
- **256Mi request**: Covers .NET runtime + application overhead
- **512Mi limit**: 2x buffer for request processing and caching
- **No over-provisioning**: Limits prevent runaway memory consumption

### 4.3 Storage Resources

| Component | Size | Type | Justification |
|-----------|------|------|---------------|
| postgres-pvc | 5Gi | standard-rwo | Sufficient for sample data; standard SSD for performance |

**Rationale:**
- **5Gi**: More than adequate for employee records (< 1MB actual data)
- **standard-rwo**: Cost-effective SSD storage; single-attach for single pod
- **Scalable**: Can expand PVC if data grows

### 4.4 Replica Configuration

| Component | Replicas | Justification |
|-----------|----------|---------------|
| employee-api | 2-10 (HPA) | Min 2 for HA; max 10 for load handling |
| postgres | 1 | Single source of truth; HA not required for demo |

**Rationale:**
- **API min 2**: Ensures availability if one pod fails
- **API max 10**: Prevents over-scaling and cost overrun
- **DB single**: Simplicity; production would use StatefulSet + replicas

---

## 5. FinOps - Cost Optimization Opportunities

### 5.1 Implemented Optimizations

| Optimization | Implementation | Impact |
|--------------|----------------|--------|
| **Right-Sized Requests** | CPU: 50m, Memory: 256Mi | Reduces wasted reserved resources |
| **HPA with Low Min Replicas** | minReplicas: 2 | Scales down during low traffic |
| **Appropriate Limits** | CPU: 500m, Memory: 512Mi | Prevents resource hogging |

### 5.2 Three Cost Optimization Opportunities

#### Opportunity 1: Use Spot/Preemptible Instances for API Tier

**Current State:** API pods run on standard on-demand nodes

**Recommendation:** Deploy API tier on Spot (preemptible) node pools

**Implementation:**
```yaml
spec:
  template:
    spec:
      nodeSelector:
        cloud.google.com/gke-spot: "true"
      tolerations:
      - key: "cloud.google.com/gke-spot"
        operator: "Equal"
        value: "true"
        effect: "NoSchedule"
```

**Estimated Savings:** 60-90% reduction in compute costs for API tier

**Risk Mitigation:** 
- HPA ensures multiple replicas
- Self-healing probes handle preemption
- Stateless design allows easy rescheduling

---

#### Opportunity 2: Implement Pod Disruption Budgets + Cluster Autoscaler

**Current State:** Fixed cluster size; manual scaling

**Recommendation:** 
- Enable Cluster Autoscaler to scale nodes based on demand
- Add PodDisruptionBudget to protect availability during scale-down

**Implementation:**
```yaml
apiVersion: policy/v1
kind: PodDisruptionBudget
metadata:
  name: employee-api-pdb
spec:
  minAvailable: 1
  selector:
    matchLabels:
      app: employee-api
```

**Estimated Savings:** 20-40% during low-traffic periods (nights, weekends)

**Benefits:**
- Automatic node provisioning/deprovisioning
- Pay only for actually needed capacity
- Maintains availability guarantees

---

#### Opportunity 3: Optimize Container Image Size

**Current State:** Multi-stage Docker build with full .NET runtime

**Recommendation:** Use Alpine-based .NET images or distroless containers

**Implementation:**
```dockerfile
# Change FROM
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base

# Or use distroless
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-jammy-chiseled AS base
```

**Estimated Savings:**
- Faster pod startup (reduced scheduling time)
- Lower container registry storage costs
- Reduced network transfer during pulls

**Image Size Comparison:**
| Image Type | Approximate Size |
|------------|------------------|
| aspnet:9.0 (Debian) | ~220 MB |
| aspnet:9.0-alpine | ~110 MB |
| runtime-deps (chiseled) | ~50 MB |

---

### 5.3 Additional Cost Recommendations

| Recommendation | Description | Priority |
|----------------|-------------|----------|
| **Resource Quotas** | Set namespace quotas to prevent over-provisioning | Medium |
| **Vertical Pod Autoscaler** | Auto-tune resource requests based on usage | Medium |
| **Scheduled Scaling** | Scale down HPA during off-hours | Low |
| **Storage Class Optimization** | Use HDD for non-critical data | Low |
| **Multi-Zone Awareness** | Single-zone deployment for non-critical workloads | Low |

### 5.4 Metrics-Based Resource Optimization

**Current HPA Configuration:**
```yaml
metrics:
- type: Resource
  resource:
    name: cpu
    target:
      type: Utilization
      averageUtilization: 70
```

**Observed Metrics:**
- CPU utilization: ~1% during idle
- Memory utilization: ~180Mi stable

**Optimization Applied:**
- Low CPU request (50m) based on observed idle usage
- Memory request (256Mi) provides headroom above observed 180Mi
- HPA manages scaling automatically based on actual demand

**Future Enhancements:**
- Add memory-based HPA metric
- Implement custom metrics (requests/second) for more accurate scaling
- Use VPA recommendations to continuously tune requests

---

## 6. Conclusion

This solution demonstrates a production-ready Kubernetes deployment following cloud-native best practices:

- **Scalability**: HPA handles traffic variations automatically
- **Reliability**: Self-healing, rolling updates, and persistence
- **Security**: Secrets management and internal-only database access  
- **Cost Efficiency**: Right-sized resources with clear optimization path
- **Maintainability**: Configuration separation and clear architecture

The implementation satisfies all assignment requirements while providing a foundation for production deployment with identified cost optimization opportunities.

---

*Document Version: 1.0*  
*Last Updated: June 2026*  
*Author: Saurabh Jain - NAGP 2026*
