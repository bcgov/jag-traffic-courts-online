create PACKAGE           occam_ords IS
    PROCEDURE create_or_replace_module;
END; 
/

create package occamords_v2 IS

  procedure create_all;

end occamords_v2;
/

create package occamords_ords_builder is

procedure info_on;
procedure debug_on;
procedure trace_on;

procedure declare_module(
    p_module_name in varchar2,
    p_base_path   in varchar2
);

procedure declare_template(
  p_pattern   in varchar2, 
  p_etag_type in varchar2 default 'HASH'
);

procedure declare_handler(
  p_procedure_name in varchar2,                   -- the qualified procedure to call
  p_method         in varchar2 default 'GET'
);

procedure declare_parameter(
  p_name               in varchar2 default null,  -- implict parameters the name will be null
  p_bind_variable_name in varchar2 default null,
  p_source_type        in varchar2 default 'URI',
  p_param_type         in varchar2 default 'STRING',
  p_access_method      in varchar2 default 'IN',
  p_implicit           in boolean default false
);

procedure declare_filter_parameter(
  p_name               in varchar2 default null,     -- implict parameters the name will be null
  p_bind_variable_name in varchar2 default null,
  p_source_type        in varchar2 default 'URI',
  p_param_type         in varchar2 default 'STRING',
  p_access_method      in varchar2 default 'IN',
  p_implicit           in boolean default false,
  p_include_like       in boolean default true       -- for non-varchar2 columns, set this to false
);

procedure declare_int_out_parameter(
  p_bind_variable_name in varchar2
);

procedure declare_implicit_parameter(
  p_bind_variable_name in varchar2
);

procedure declare_refcursor_out_parameter(
  p_name               in varchar2
);

procedure define_module;  

end occamords_ords_builder;
/

