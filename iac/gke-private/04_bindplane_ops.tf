resource "kubernetes_namespace" "bindplane" {
  metadata {
    name = "bindplane"
  }
}

module "bindplane-secret" {
  source = "terraform-google-modules/gcloud/google//modules/kubectl-wrapper"

  project_id              = var.project_id
  cluster_name            = module.gke.name
  cluster_location        = module.gke.location
  kubectl_create_command  = "kubectl apply -f bindplane-manifests/ -n ${kubernetes_namespace.bindplane.id}" 
  kubectl_destroy_command = "echo done"
}

resource "helm_release" "bindplane" {
  name       = "bindplane"
  namespace  = kubernetes_namespace.bindplane.id
  repository = "https://observiq.github.io/bindplane-op-helm"
  chart      = "bindplane"
}
