# module "asm" {
#   source            = "terraform-google-modules/kubernetes-engine/google//modules/asm"
#   project_id        = var.project_id
#   cluster_name      = module.gke.name
#   cluster_location  = module.gke.location
#   module_depends_on = [module.fleet.wait]
#   #fleet_id                  = module.fleet.cluster_membership_id
#   #internal_ip               = true
#   enable_cni                = false
#   enable_mesh_feature       = true
# }

resource "kubernetes_namespace" "gateway" {
  metadata {
    labels = {
      "service" = "gateway",
      "istio-injection" = "enabled"
    }
    name = "gateway"
  }
}

module "istio-gateway" {
  source = "terraform-google-modules/gcloud/google//modules/kubectl-wrapper"

  project_id              = var.project_id
  cluster_name            = module.gke.name
  cluster_location        = module.gke.location
  #module_depends_on       = [module.asm.wait]
  kubectl_create_command  = "kubectl apply -f istio-ingressgateway/deployment.yaml -n ${kubernetes_namespace.gateway.id}" # && kubectl apply -f istio-ingressgateway/cloud-trace.yaml"
  kubectl_destroy_command = "echo done"
}
