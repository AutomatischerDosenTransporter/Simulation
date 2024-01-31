FROM registry.nussi.net/ros:foxy-desktop

RUN apt update -y
RUN apt install ros-foxy-joint-state-publisher -y
# RUN echo 'source /opt/ros/humble/setup.bash' >> ~/.bashrc 
ENTRYPOINT ["/ros_entrypoint.sh"]