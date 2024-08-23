module "fleet" {
  source     = "terraform-google-modules/kubernetes-engine/google//modules/fleet-membership"
  project_id = var.project_id
  location   = module.gke.location
  membership_location = module.gke.location
  cluster_name = module.gke.name
  enable_fleet_registration = true
}

