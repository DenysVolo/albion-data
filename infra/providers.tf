terraform {
    required_providers {
        aws = {
            source  = "hashicorp/aws"
            version = "~> 3.27"
        }
    }
}

provider "aws" {
    region    = "eu-west-2"
    access_key = "user_access_key"
    secret_key = "user_secret"
}