terraform {
  required_providers {
    google = {
      version = "> 5.9.0"
    }
    google-beta = {
      version = "> 5.9.0"
    }
  }
}
provider "google" {
  project = var.project_id
  region  = var.region
  credentials = "${file(var.iac_servicekey_path)}"
}

provider "google-beta" {
  project = var.project_id
  region  = var.region
  credentials = "${file(var.iac_servicekey_path)}"
}

data "google_client_config" "default" {}

provider "kubernetes" {
  host                   = "https://${module.gke.endpoint}"
  token                  = data.google_client_config.default.access_token
  cluster_ca_certificate = base64decode(module.gke.ca_certificate)
}

provider "helm" {
  kubernetes {
    host                   = "https://${module.gke.endpoint}"
    token                  = data.google_client_config.default.access_token
    cluster_ca_certificate = base64decode(module.gke.ca_certificate)  
  }
}
