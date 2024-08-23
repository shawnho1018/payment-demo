variable "project_id" {
  type = string 
  description = "Project to store terraform state variables"
}
variable "region" {
  type = string
}
variable "zones" {
  description = "The primary zones to be used"
  default = ["asia-east1-a", "asia-east1-b", "asia-east1-c"]
}
variable "cluster_name" {
  description = "cluster name used in the lab"
}

variable "vm_type" {
  default = "n1-standard-4"
}
variable "network" {
  type = string
  default = "mesh-network"
}
variable "subnet" {
  type = string
  default = "mesh-subnet"
}

variable "master_ipv4_cidr_block" {
  type = string
  default = "10.4.1.0/28"
}
variable "iac_servicekey_path" {
  type = string
  description = "Path to the GCP service key file which will be used for crossplane"
}


variable "bindplane_username" {
  type = string
}

variable "bindplane_password" {
  type = string
}

variable "ip_range_pods" {
  type = string
}

variable "ip_range_services" {
  type = string
}
