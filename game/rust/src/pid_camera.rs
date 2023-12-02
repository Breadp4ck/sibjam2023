use godot::prelude::*;

#[derive(GodotClass)]
#[class(base=Node3D)]
struct PidController {
    integral_sum: Vector3,
    // last_value: Vector3,
    // set: bool,
    #[export]
    p: f32,
    #[export]
    i: f32,
    // #[export]
    // d: f32,
    #[export]
    parent: Option<Gd<Node3D>>,
    #[export]
    object: Option<Gd<Node3D>>,
    #[export]
    set_rotation: bool,

    #[base]
    base: Base<Node3D>,
}

#[godot_api]
impl PidController {}

#[godot_api]
impl INode3D for PidController {
    fn init(base: Base<Node3D>) -> Self {
        Self {
            integral_sum: Vector3::ZERO,
            // last_value: Vector3::ZERO,
            // set: false,
            p: 20.0,
            i: 10.0,
            // d: 0.0,
            parent: None,
            object: None,
            base,
            set_rotation: true,
        }
    }

    fn physics_process(&mut self, delta: f64) {
        let Some(ref parent) = self.parent else {
            return;
        };
        let Some(ref mut object) = self.object else {
            return;
        };

        if self.set_rotation {
            object.set_rotation(parent.get_rotation());
        }

        let object_pos = object.get_global_position();
        let parent_pos = parent.get_global_position();

        let current_error = (parent_pos - object_pos) * delta as f32;

        self.integral_sum += current_error;

        let output =
            Vector3::splat(self.p) * current_error + Vector3::splat(self.i) * self.integral_sum;
        // - if self.set {
        //     Vector3::splat(self.d) * (parent_pos - self.last_value) // delta as f32
        // } else {
        //     self.set = true;
        //     Vector3::splat(0.0)
        // };

        // self.last_value = parent_pos;
        object.set_global_position(object_pos + output);
    }
}
