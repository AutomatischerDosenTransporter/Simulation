FROM registry.nussi.net/ros:foxy-desktop

RUN apt update -y
RUN apt install -y ros-foxy-xacro
RUN apt install -y ros-foxy-ros2-control
RUN apt install -y ros-foxy-ros2-controllers

ENTRYPOINT ["/ros_entrypoint.sh"]