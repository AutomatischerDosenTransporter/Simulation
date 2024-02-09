FROM registry.nussi.net/ros:foxy-desktop

RUN apt update -y
RUN apt install -y ros-foxy-xacro
RUN apt install -y ros-foxy-navigation2
RUN apt install -y ros-foxy-nav2-bringup

ENTRYPOINT ["/ros_entrypoint.sh"]