FROM registry.nussi.net/ros:foxy-desktop

RUN apt update -y
RUN apt install -y ros-foxy-xacro
RUN apt install -y ros-foxy-joint-state-publisher

ENTRYPOINT ["/ros_entrypoint.sh"]