/**
 * @param {string} ref
 * @returns {string}
 */
const removeDtoFromRef = (ref) => {
  return ref.replace(/Dto(["']|$|#)/g, '$1');
};

/**
 * @param {any} obj
 * @returns {any}
 */
const transformRefs = (obj) => {
  if (!obj || typeof obj !== 'object') return obj;

  if (Array.isArray(obj)) {
    return obj.map(transformRefs);
  }

  const newObj = {};

  for (const [key, value] of Object.entries(obj)) {
    if (key === '$ref' && typeof value === 'string') {
      newObj[key] = removeDtoFromRef(value);
    } else {
      newObj[key] = transformRefs(value);
    }
  }

  return newObj;
};

/**
 * @param {import('openapi3-ts/oas31').OpenAPIObject} inputSpec
 * @returns {import('openapi3-ts/oas31').OpenAPIObject}
 */
export default (inputSpec) => {
  const spec = { ...inputSpec };

  // Transform schema names
  if (spec.components?.schemas) {
    const newSchemas = {};

    Object.entries(spec.components.schemas).forEach(([key, value]) => {
      const newKey = key.replace(/Dto$/, '');
      newSchemas[newKey] = value;
    });

    spec.components = {
      ...spec.components,
      schemas: newSchemas
    };
  }

  // Transform all $ref occurrences throughout the spec
  return transformRefs(spec);
};
