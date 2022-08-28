# EFSintegration - EKSFargate
This is to demonstrate steps to mount **Elastic File System(EFS)** in **AWS EKS-Fargate container** and run the .Net Core application container which uploads a file to the mounting file share system.

## Overview
This demonstration illustrates how to mount a Persistent storage *Elastic File System (EFS)* in *Elastic Kubernetes System* using **Fargate**. The example comprises a .Net core API, to upload a document to the file share and AWS CLI commands to mount EFS. This example does not include steps to set up an EKS Cluster and ALB to run the application, however, added relevant yaml files that can be used while setting up the cluster. In addition, you may have to include all the IAM policies that will help to run the Cluster. All other files and commands to set up the EFS will find the following paragraph. 
