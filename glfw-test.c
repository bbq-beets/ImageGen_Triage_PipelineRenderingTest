#include <GLFW/glfw3.h>
#include <stdio.h>

int main(void) {
    if (!glfwInit()) {
        printf("GLFW initialization failed!\n");
        return 1;
    }
    GLFWwindow* window = glfwCreateWindow(300, 200, "GLFW Test", NULL, NULL);
    if (!window) {
        printf("GLFW window creation failed!\n");
        glfwTerminate();
        return 1;
    }
    printf("GLFW initialized and window created successfully.\n");
    glfwDestroyWindow(window);
    glfwTerminate();
    return 0;
}
