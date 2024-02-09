FROM registry.nussi.net/ros:foxy-desktop

RUN apt update -y
RUN apt install -y ros-foxy-xacro

ENTRYPOINT ["/ros_entrypoint.sh"]