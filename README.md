# EFSintegration - EKSFargate
This is to demonstrate steps to mount **Elastic File System(EFS)** in **AWS EKS-Fargate container** and run the .Net Core application container which uploads a file to the mounting file share system.

## Overview
This illustrates how to mount a Persistent storage Elastic File System (EFS) in Elastic Kubernetes System using Fargate. The example comprises a .Net core API, to upload a document to the file share and AWS CLI commands to mount EFS. This does not include steps to set up an EKS Cluster and ALB to run the application, however, added relevant YAML files that will be useful while setting up the cluster. In addition, you may have to include all the IAM policies that will help to run the cluster, and the rest of the commands to set up the EFS will be added to the following lines.

EFS is a simple scalable, and fully managed shared file system for use with AWS and started supporting the EKS cluster using Fargate in 2020. EFS also supports multi-zone, as well as multi-region, read-write support, and all the data written will be available in all the availability zones. The EFS CSI driver makes it simple to configure elastic file storage for Kubernetes clusters, and before this update, customers could use EFS via Amazon EC2 worker nodes connected to a cluster. Now customers can also configure their pods running on Fargate to access an EFS file system using standard Kubernetes APIs. With this update, customers can run stateful workloads that require highly available file systems as well as workloads that require access to shared storage. Using the EFS CSI driver, all data in transit is encrypted by default.

Persistent volume can be set up by both dynamic and static provisioning and Fargate does not support dynamic provisioning whereas,  a pod running on AWS Fargate automatically mounts an Amazon EFS file system, without needing the manual driver installation steps described on this page thus, we do not have to install the EFS driver manually. For more information, you can find the documents here.

> *NB: EFS Drivers will not support windows container*

## Prerequisites
1) AWS account
2) An existing EKS cluster with Kubernetes version 1.22
3) Necessary IAM Roles and Policies set for cluster and fargate pod execution
4) eksctl - 0.105.0
5) aws-cli 2.7.12

## Commands

Follow the below commands to setup both EFS and mounting to a cluster.

> Following commands are going to be run on Powershell, thus variable sections may included.

1) Retrieve cluster VPCId and assign to a variable
   ```
   $vpc_id=$(aws eks describe-cluster --name ekstest --query "cluster.resourcesVpcConfig.vpcId" --output text)
   
   ```
2) Assign CIDR range associated to the VPC
   ```
   $cidr_range=$(aws ec2 describe-vpcs --vpc-ids $vpc_id --query "Vpcs[].CidrBlock" --output text)
   
   ```
3) Create separate security group under the same VPC
   ```
   $security_group_id=$(aws ec2 create-security-group --group-name EfsSecurityGroup1 --description "My EFS security group" --vpc-id $vpc_id --output text)
   
   ```
4) Adding inboud rule in newly created SG for port 2049 and Type NFS to communicate with EFS
   ```
   aws ec2 authorize-security-group-ingress --group-id $security_group_id --protocol tcp --port 2049 --cidr $cidr_range
   
   ```
   the following command will give an output on successful execution
   ```
   {
    "Return": true,
    "SecurityGroupRules": [
        {
            "SecurityGroupRuleId": "sgr-1111111111",
            "GroupId": "sg-097d6a30e942711111",
            "GroupOwnerId": "000000000",
            "IsEgress": false,
            "IpProtocol": "tcp",
            "FromPort": 2049,
            "ToPort": 2049,
            "CidrIpv4": "192.168.0.0/16"
        }
     ]
    }
   ```
5) Create an Amazon EFS file system for your Amazon EKS cluster.
   ```
   $file_system_id=$(aws efs create-file-system --region us-east-1 --performance-mode generalPurpose --query 'FileSystemId' --output text)
   
   ```
6) Create EFS Mount target
   ```
   # List available subnet
   aws ec2 describe-subnets --filters "Name=vpc-id,Values=$vpc_id" --query 'Subnets[*].{SubnetId: SubnetId,AvailabilityZone: AvailabilityZone,CidrBlock: CidrBlock}' --output table
   
   # Add each Private subnet to the mount target
   aws efs create-mount-target --file-system-id $file_system_id --subnet-id subnet-1111111111111 --security-groups $security_group_id
   ```
