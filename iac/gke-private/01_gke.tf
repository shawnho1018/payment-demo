data "google_compute_network" "default_network" {
  name    = var.network
  project = var.project_id
}
data "google_compute_subnetwork" "subnetwork" {
  name    = var.subnet
  project = var.project_id
  region  = var.region
}

module "gke" {
  source                     = "terraform-google-modules/kubernetes-engine/google//modules/private-cluster"
  project_id                 = data.google_project.project.project_id
  name                       = var.cluster_name
  region                     = data.google_compute_subnetwork.subnetwork.region
  zones                      = var.zones
  network                    = data.google_compute_network.default_network.name
  subnetwork                 = data.google_compute_subnetwork.subnetwork.name
  network_project_id         = var.project_id
  ip_range_pods              = data.google_compute_subnetwork.subnetwork.secondary_ip_range[0].range_name
  ip_range_services          = data.google_compute_subnetwork.subnetwork.secondary_ip_range[1].range_name
  http_load_balancing        = true
  horizontal_pod_autoscaling = true
  filestore_csi_driver       = true
  enable_vertical_pod_autoscaling = true
  datapath_provider          = "ADVANCED_DATAPATH"
  enable_binary_authorization = true
  enable_shielded_nodes      = true
  enable_cost_allocation     = true
  enable_network_egress_export = true
  enable_resource_consumption_export = true
  resource_usage_export_dataset_id = google_bigquery_dataset.gke_consumption.dataset_id
  enable_private_endpoint    = false
  enable_private_nodes       = true
  gke_backup_agent_config    = true
  grant_registry_access      = true
  master_ipv4_cidr_block     = var.master_ipv4_cidr_block
  cluster_dns_provider       = "CLOUD_DNS"
  cluster_dns_scope          = "CLUSTER_SCOPE"
  cluster_resource_labels = { "mesh_id" : "proj-${data.google_project.project.number}" }
  gateway_api_channel        = "CHANNEL_STANDARD"
  monitoring_enable_managed_prometheus = true
  monitoring_enabled_components = ["SYSTEM_COMPONENTS", "APISERVER", "CONTROLLER_MANAGER", "SCHEDULER"]

  release_channel            = "REGULAR"
  deletion_protection        = false 
  node_pools = [
    {
      name                      = "default-node-pool"
      machine_type              = var.vm_type
      node_locations            = "asia-east1-b"
      min_count                 = 1
      max_count                 = 3
      local_ssd_count           = 0
      spot                      = true
      disk_size_gb              = 100
      disk_type                 = "pd-standard"
      image_type                = "COS_CONTAINERD"
      enable_gcfs               = true
      enable_gvnic              = false
      auto_repair               = true
      auto_upgrade              = true
      preemptible               = false
      initial_node_count        = 1
    } 
  ]
  node_pools_oauth_scopes = {
    all = [
      "https://www.googleapis.com/auth/cloud-platform"
    ]
  }

  node_pools_labels = {
    all = {}
    default-node-pool = {
      default-node-pool = true
      spot-instance     = true
      belong-to         = var.cluster_name
      
    }
  }

  node_pools_metadata = {
    all = {}
    default-node-pool = {
      node-pool-metadata-custom-value = "default-node-pool"
    }
  }

  node_pools_taints = {
    all = []

    default-node-pool = [
      {
        key    = "default-node-pool"
        value  = true
        effect = "PREFER_NO_SCHEDULE"
      },
    ]
  }
  node_pools_resource_labels = {
    default-node-pool = {
      user = "shawn"
      purpose = "demo"
    }
  }
  node_pools_tags = {
    all = []

    default-node-pool = [
      "default-node-pool",
      "http-server",
    ]
  }
  master_authorized_networks = [
    {
       cidr_block   = "0.0.0.0/0"
       display_name = "VPC"
    } 
  ]
}
