use godot::{engine::InputEvent, prelude::*};

#[derive(GodotClass)]
#[class(base=Node)]
struct FiniteStateMachine {
    #[export]
    initial_state: Option<Gd<FiniteState>>,

    state: Gd<FiniteState>,
    context: Gd<FiniteStateContext>,

    #[base]
    base: Base<Node>,
}

#[godot_api]
impl FiniteStateMachine {
    #[func]
    fn transition_to(&mut self, next_state: String) {
        if let Some(node) = self.base.get_node(next_state.into()) {
            if let Ok(state) = node.try_cast::<FiniteState>() {
                self.call_state_end();
                self.state = state;
                self.call_state_start();
            }
        }
    }

    #[inline]
    fn call_state_start(&mut self) {
        if self.state.has_method("_start".into()) {
            self.state
                .call("_start".into(), &[self.context.to_variant()]);
        }
    }

    #[inline]
    fn call_state_end(&mut self) {
        if self.state.has_method("_end".into()) {
            self.state.call("_end".into(), &[self.context.to_variant()]);
        }
    }

    #[inline]
    fn call_state_update(&mut self, delta: f64) {
        if self.state.has_method("_update".into()) {
            self.state.call(
                "_update".into(),
                &[self.context.to_variant(), Variant::from(delta)],
            );
        }
    }

    #[inline]
    fn call_state_handle_input(&mut self, input: Gd<InputEvent>) {
        if self.state.has_method("_handle_input".into()) {
            self.state.call(
                "_handle_input".into(),
                &[self.context.to_variant(), input.to_variant()],
            );
        }
    }

    #[inline]
    fn call_state_physics_update(&mut self, delta: f64) {
        if self.state.has_method("_physics_update".into()) {
            self.state.call(
                "_physics_update".into(),
                &[self.context.to_variant(), Variant::from(delta)],
            );
        }
    }

    #[inline]
    fn proceed_context(&mut self) {
        let next_state = {
            let context = &mut self.context.bind_mut();

            let next_state = if let Some(state) = &mut context.next_state {
                state.clone()
            } else {
                return;
            };

            context.next_state = None;
            next_state
        };

        self.transition_to(next_state);
    }
}

#[godot_api]
impl INode for FiniteStateMachine {
    fn init(base: Base<Node>) -> Self {
        Self {
            initial_state: None,
            state: FiniteState::alloc_gd(),
            context: FiniteStateContext::alloc_gd(),
            base,
        }
    }

    fn ready(&mut self) {
        if self.initial_state == None {
            panic!("Specify initial state for FiniteStateMachine node!");
        }

        self.state = self.initial_state.as_mut().unwrap().clone();
        self.call_state_start();
        self.proceed_context();
    }

    fn input(&mut self, input: Gd<InputEvent>) {
        self.call_state_handle_input(input);
        self.proceed_context();
    }

    fn process(&mut self, delta: f64) {
        self.call_state_update(delta);
        self.proceed_context();
    }

    fn physics_process(&mut self, delta: f64) {
        self.call_state_physics_update(delta);
        self.proceed_context();
    }
}

#[derive(GodotClass)]
#[class(base=Node)]
struct FiniteState;

#[godot_api]
impl FiniteState {}

#[godot_api]
impl INode for FiniteState {
    fn init(_: Base<Node>) -> Self {
        Self
    }
}

#[derive(GodotClass)]
#[class(base=Node)]
struct FiniteStateContext {
    next_state: Option<String>,
}

#[godot_api]
impl FiniteStateContext {
    #[func]
    /// Jump to new state by name
    fn jump_to(&mut self, state_name: String) {
        self.next_state = Some(state_name);
    }
}

#[godot_api]
impl INode for FiniteStateContext {
    fn init(_: Base<Node>) -> Self {
        Self { next_state: None }
    }
}
