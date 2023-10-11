#!/bin/bash
exec > >(tee /var/log/user-data.log | logger -t user-data -s 2>/dev/console) 2>&1

echo "Configuring ECS agent..."
echo "ECS_CLUSTER=${ecs_cluster_name}" >>/etc/ecs/ecs.config
echo "ECS_ENABLE_CONTAINER_METADATA=true" >>/etc/ecs/ecs.config
echo "ECS_ENGINE_TASK_CLEANUP_WAIT_DURATION=3m" >>/etc/ecs/ecs.config

echo "######    Installing CloudWatch Agent..."
yum install wget -y
wget https://s3.amazonaws.com/amazoncloudwatch-agent/amazon_linux/amd64/latest/amazon-cloudwatch-agent.rpm
rpm -U ./amazon-cloudwatch-agent.rpm
rm -f amazon-cloudwatch-agent.rpm
/opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-ctl -m ec2 -a start

echo "######    Installing additional packages..."
yum install -y unzip perl-Switch perl-DateTime perl-Sys-Syslog perl-LWP-Protocol-https perl-Digest-SHA.x86_64
yum install -y https://s3.amazonaws.com/ec2-downloads-windows/SSMAgent/latest/linux_amd64/amazon-ssm-agent.rpm

echo "######    Configuring custom monitoring scripts..."
cd /root/
curl https://aws-cloudwatch.s3.amazonaws.com/downloads/CloudWatchMonitoringScripts-1.2.2.zip -O
unzip CloudWatchMonitoringScripts-1.2.2.zip
rm CloudWatchMonitoringScripts-1.2.2.zip

echo "######    Sending custom metrics to CloudWatch..."
(
    crontab -l
    echo "*/5 * * * * /root/aws-scripts-mon/mon-put-instance-data.pl --disk-path=\"${disk_path}\" --disk-space-util --disk-space-used --disk-space-avail --disk-space-units=gigabytes --from-cron"
    echo "*/5 * * * * /root/aws-scripts-mon/mon-put-instance-data.pl --mem-util --mem-used --mem-used-incl-cache-buff --mem-avail --memory-units=gigabytes --from-cron"
) | crontab -

echo "######    Sending userdata logs to CloudWatch..."
cat <<EOF >/opt/aws/amazon-cloudwatch-agent/etc/amazon-cloudwatch-agent.json
{
    "agent": {
        "metrics_collection_interval": 60,
        "run_as_user": "cwagent",
        "logfile": "/opt/aws/amazon-cloudwatch-agent/logs/amazon-cloudwatch-agent.log"
    },
    "logs": {
        "logs_collected": {
            "files": {
                "collect_list": [
                    {
                        "file_path": "/var/log/user-data.log",
                        "log_group_name": "${maintenance_log_group_name}",
                        "log_stream_name": "{instance_id}-{ip_address}/var/log/user-data.log",
                        "timestamp_format": "%b %d %H:%M:%S"
                    }
                ]
            }
        }
    }
}
EOF

/opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-ctl -m ec2 -a stop
/opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-ctl -m ec2 -a start

echo "######    Running additional user data script..."
${additional_user_data_script}
